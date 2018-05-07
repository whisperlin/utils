#pragma once
#include <string>
#include <vector>
int string_split(std::vector<std::string>& dst, const std::string& src, const std::string& separator);
std::string string_remove_dot_dot(std::string path);
std::string string_relative_path(std::string path1, std::string path2);
std::string   replace_all(std::string   str, const   std::string&   old_value, const   std::string&   new_value);