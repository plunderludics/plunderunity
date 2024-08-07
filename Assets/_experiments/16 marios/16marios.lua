local DIR = ".\\Assets\\StreamingAssets\\"
-- local DIR = "./plunder_Data/StreamingAssets"
local lib = require(DIR.."scripts/lib/lib")
local json = require(DIR.."scripts/lib/json")
local plunder = require(DIR.."scripts/lib/plunder")

plunder.USE_SERVER = true
plunder.USE_SERVER_INPUT = true

frameTimer = 0
SHOW_DEBUG = true

-- this is used externally to send information to this window
windowName = "untitled"

CURR_GAME = "mario";

-- if there's multiple instances with same name, they can be used to distinguish them
instance = 0

local currSample
local isPaused

local lastHealth = 8

function main()
	print("starting lua stuff")
	-- loop forever waiting for clients
	print("starting rom: ", gameinfo.getromname())
	domainToCheck = "RDRAM"
	currMem = {}

	-- TODO: maybe wait for first load
	-- TODO: maybe send loading state

	-- https://github.com/TASEmulators/BizHawk/issues/1141#issuecomment-410577001
	started = false
    unityhawk.callmethod("OnLoad", "")
	while true do
		gui.clearGraphics()
		local f = emu.framecount()

		if not isPaused then
			emu.frameadvance()
		else
		    emu.yield()
		end

		if frameTimer >= 0 then
			frameTimer = frameTimer + 1
		end
		if frameTimer > 60 then
			frameTimer = 0
		end

		-- prevent mario from getting frozen in dialogue
		local newMem = plunder.readMemory(CURR_GAME)

		local health = newMem.health

		if lastHealth > 0 and health == 0 then
			local _ = unityhawk.callmethod("OnDeath", "")
			print("mario died")
		end

		if lastHealth == 0 and health > 0 then
			local _ = unityhawk.callmethod("OnRespawn", "")
			print("mario respawned")
		end

		lastHealth = health

		local isOnTree = false
		if newMem.phase == 135267136 or (newMem.phase > 1049408 and newMem.phase < 1049414) then
			isOnTree = true
		-- else
			-- print("not tree")
		end


		local _ = unityhawk.callmethod("IsOnTree", isOnTree and "true" or "false")
		local _ = unityhawk.callmethod("GetState", json.encode(newMem))
		local extMemStr = unityhawk.callmethod("SetState", "");
		if extMemStr ~= nil and extMemStr ~= "" then
			local extMem = json.decode(extMemStr)
			plunder.writeMemory(CURR_GAME, extMem)
		end

		local healthStr = unityhawk.callmethod("SetHealth", "");
		if healthStr ~= nil and healthStr ~= "" then
			plunder.setValue(plunder.MEM[CURR_GAME].health, tonumber(healthStr))
		end

		if SHOW_DEBUG then
			gui.text(10, 10, dbg)
		end
	end
end


main()