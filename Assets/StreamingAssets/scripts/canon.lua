local lib = require("./Assets/StreamingAssets/scripts/lib/lib")
local plunder = require("./Assets/StreamingAssets/scripts/lib/plunder")

plunder.USE_SERVER = true
plunder.USE_SERVER_INPUT = true

frameTimer = 0
SHOW_DEBUG = true

local currSample
local isPaused

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
  -- unityhawk.callmethod("OnLoad", "")
	while true do
		gui.clearGraphics()
		local f = emu.framecount()

		-- local pauseStr = unityhawk.callmethod("Pause", "")
		-- local shouldPause = pauseStr == "true"
		-- if (not isPaused and shouldPause) then
		-- 		isPaused = true
		-- elseif (isPaused and not shouldPause) then
		-- 		isPaused = false
		-- end

		-- local sample = unityhawk.callmethod("LoadSample", ""..f)
		-- if (sample ~= "none" and sample ~= currSample) then
    --         console.log("[unity] loading sample "..sample)
		-- 	plunder.runSample(sample)
    --         unityhawk.callmethod("OnLoadedSample", sample..","..f)
    --         currSample = sample
		-- end

    -- local volumeStr = unityhawk.callmethod("SetVolume", "")
    -- if (volumeStr) then
    --     local volume = tonumber(volumeStr)
    --     client.SetVolume(volume);
    -- end

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

    local analogStr = unityhawk.callmethod("SetAnalog", "")
		if (analogStr) then
      local s = split(analogStr, ",")
      local x = tonumber(s[1])
      local y = tonumber(s[2])
      joypad.setanalog({
				['X Axis'] = x and tostring(math.floor(x * 127)) or '',
				['Y Axis'] = y and tostring(math.floor(y * 127)) or '',
			}, 1)
		end

		if SHOW_DEBUG then
			gui.text(10, 10, dbg)
		end
	end
end

function split(pString, pPattern)
   local Table = {}  -- NOTE: use {n = 0} in Lua-5.0
   local fpat = "(.-)" .. pPattern
   local last_end = 1
   local s, e, cap = pString:find(fpat, 1)
   while s do
      if s ~= 1 or cap ~= "" then
     table.insert(Table,cap)
      end
      last_end = e+1
      s, e, cap = pString:find(fpat, last_end)
   end
   if last_end <= #pString then
      cap = pString:sub(last_end)
      table.insert(Table, cap)
   end
   return Table
end


main()