local vfs_c = require "ejoy2dx.vfs.c"

local utils = {}



--do_undo_system
function print_dump(data, showMetatable, lastCount)
    if type(data) ~= "table" then
        --Value
        if type(data) == "string" then
            io.write("\"", data, "\"")
        else
            io.write(tostring(data))
        end
    else
        --Format
        local count = lastCount or 0
        count = count + 1
        io.write("{\n")
        --Metatable
        if showMetatable then
            for i = 1,count do 
                io.write("\t") 
            end
            local mt = getmetatable(data)
            io.write("\"__metatable\" = ")
            print_dump(mt, showMetatable, count)    -- 如果不想看到元表的元表，可将showMetatable处填nil
            io.write(",\n")        --如果不想在元表后加逗号，可以删除这里的逗号
        end
        --Key
        for key,value in pairs(data) do
            for i = 1,count do 
                io.write("\t") 
            end
            if type(key) == "string" then
                io.write("\"", key, "\" = ")
            elseif type(key) == "number" then
                io.write("[", key, "] = ")
            else
                io.write(tostring(key))
            end
            print_dump(value, showMetatable, count)    -- 如果不想看到子table的元表，可将showMetatable处填nil
            io.write(",\n")        --如果不想在table的每一个item后加逗号，可以删除这里的逗号
        end
        --Format
        for i = 1,lastCount or 0 do 
            io.write("\t") 
        end
            io.write("}")
    end
    --Format
    if not lastCount then
        io.write("\n")
    end
end
function utils.path_combine(...)
	local tmp = {}
	for _, path in ipairs({...}) do
		if (path ~= "") then
			table.insert(tmp, path)
		end
	end
	local ret = string.gsub(string.gsub(table.concat(tmp, "/"), "\\", "/"), "(/+)", "/")
	return ret
end

function utils.remove_dotdot(path)
	local tmp = {}
	for name in string.gmatch(path, "([^/\\]+)") do
		if (name ~= ".") then
			if (name == "..") then
				table.remove(tmp)
			else
				table.insert(tmp, name)
			end
		end
	end
	
	return table.concat(tmp, "/")
end

function utils.relative_path(fullpath, root)
	local fullpath = utils.remove_dotdot(fullpath)
	local root = utils.remove_dotdot(root)

	local relative = string.gsub(fullpath, root, "")
	if (relative and string.byte(relative, 1) == 47) then	-- '/'
		return string.sub(relative, 2)
	end

	return relative
end

function trim (s) 
  return (string.gsub(s, "^%s*(.-)%s*$", "%1")) 
end


function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter=='') then return false end
    local pos,arr = 0, {}
    for st,sp in function() return string.find(input, delimiter, pos, true) end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end
function utils.load_conf_abs(filepath)

	local file = vfs_c.readfile("abs:"..filepath)
	if (not file) then
		return
	end
	file= string.gsub(file, "\r", "") 
	lines = string.split(file,"\n")
	local scheme = {}
	local curr_section
	local curr_section_key

	for i = 1 , #lines do
		line = lines[i]
		--print("line = ","|"..line.."|")
		local section = line:match("^%[([^%[%]]+)%]%s*$")
		if(section)then
			scheme[section] = scheme[section] or {}
			curr_section_key = section
			curr_section = scheme[section]
			print_dump(curr_section)
		else
			if (not curr_section) then
				return nil
			end
			local key, value = line:match('^([%w_]+)%s*=%s*(.+)%s*$')
			 
			if (key and value) then
				curr_section[key] = value
				--print(curr_section_key,":",key,"=","|"..value.."|")
				--curr_section[key] = trim(value)
			end
		end
	end


	 
	return scheme
end

function utils.load_conf(filepath)
	--print("utils.load_conf",filepath)
	--print(debug.traceback())
	local file = io.open(filepath, 'r')
	if (not file) then
		--print("not file")
		return
	end

	local scheme = {}
	local curr_section
	local curr_section_key
	for line in file:lines() do
		--print("line = ",line)
		local section = line:match("^%[([^%[%]]+)%]%s*$")
		if(section)then
			scheme[section] = scheme[section] or {}
			curr_section_key = section
			curr_section = scheme[section]
			print_dump(curr_section)
		else
			if (not curr_section) then
				--print("conf file format error")
				file:close()
				--print("curr_section = ",curr_section)
				return nil
			end
			local key, value = line:match('^([%w_]+)%s*=%s*(.+)%s*$')
			 
			if (key and value) then
				curr_section[key] = value
				--curr_section[key] = trim(value)
			end
		end
	end

	file:close()
	 
	return scheme
end

function utils.save_conf(filepath, data)
	assert(type(data) == "table")
	local file = io.open(filepath, "w+b")
	assert(file)

	local contents = ""
	for section, param in pairs(data) do
		contents = contents .. string.format("[%s]\r\n", section)
		for key, value in pairs(param) do
			contents = contents .. string.format("%s = %s\r\n", key, tostring(value))
		end
		contents = contents .. "\r\n"
	end

	file:write(contents)
	file:close()
end

function utils.check_file_exist(filename)
	local m = io.open(filename)
	if (not m) then
		return nil
	else
		m:close()
	end

	return true
end

function utils.file_size(filename)
	local m = io.open(filename)
	if (not m) then
		return 0
	end

	local size = m:seek("end", 0)
	m:close()

	return size
end

return utils
