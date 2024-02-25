local lib = {}
function lib.split (inputstr, sep)
   if sep == nil then
      sep = "%s"
   end
   local t={}
   for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
      table.insert(t, str)
   end
   return t
end

function lib.last(arr)
    return arr[#arr]
end

function lib.get_keys(t)
   local keys={}
   for key,_ in pairs(t) do
     table.insert(keys, key)
   end
   return keys
end

function lib.file_exists(file)
  local f = io.open(file, "rb")
  if f then f:close() end
  return f ~= nil
end

function lib.lines_from(file)
  if not lib.file_exists(file) then return {} end
  local lines = {}
  for line in io.lines(file) do
    lines[#lines + 1] = line
  end
  return lines
end

function lib.lerp(a, b, v)
  return a + (b-a) * v
end

function lib.inverselerp(a, b, v)
  return (v-a) / (b-a)
end

function lib.remap(a, b, x, y, v)
  return lib.inverselerp(x, y, lerp(a, b, v))
end

return lib