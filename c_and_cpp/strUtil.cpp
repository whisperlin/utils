
#include "strUtil.h"
#include "strUtil.hpp"
#include "string_extern.h"
#include <string>
#include <vector>




std::string string_combine_path(std::string  path0, std::string path1)
{
	path0 = replace_all(path0, "\\", "/");
	path1 = replace_all(path1, "\\", "/");
	int len = path0.size();
	if (len > 0 && path0.c_str()[len - 1] != '/')
	{
		path0 = path0 + "/";
	}
	path0 = path0 + path1;
	path0 = replace_all(path0, "//", "/");
	path0 = string_remove_dot_dot(path0);
	return path0;
}
int string_split(std::vector<std::string>& dst, const std::string& src, const std::string& separator)
{
	if (src.empty() || separator.empty())
		return 0;

	int nCount = 0;
	std::string temp;
	size_t pos = 0, offset = 0;

	// 分割第1~n-1个
	while ((pos = src.find_first_of(separator, offset)) != std::string::npos)
	{
		temp = src.substr(offset, pos - offset);
		if (temp.length() > 0) {
			dst.push_back(temp);
			nCount++;
		}
		offset = pos + 1;
	}

	// 分割第n个
	temp = src.substr(offset, src.length() - offset);
	if (temp.length() > 0) {
		dst.push_back(temp);
		nCount++;
	}

	return nCount;
}
std::string string_remove_dot_dot(std::string path)
{
	std::vector<std::string> res;
	string_split(res, path, "\\/");
	int len = res.size();
	std::vector<std::string> res2;
	int res2size = 0;
	for (int i = 0; i < len; i++)
	{
		if (res[i].compare("..") == 0)
		{
			if (res2size > 0)
			{
				res2.pop_back();
				res2size--;
				continue;
			}
		}
		else if (res[i].size() == 0 || res[i].compare(".") == 0)
		{
			continue;
		}
		res2.push_back(res[i]);
		res2size++;
	}
	if (res2size == 0)
	{
		return path;
	}
	else
	{
		std::string _path = res2[0];
		for (int i = 1; i < res2size; i++)
		{
			_path = _path + "/" + res2[i];
		}
		return _path;
	}

}

std::string string_relative_path(std::string fullpath, std::string root)
{
	fullpath = string_remove_dot_dot(fullpath);
	root = string_remove_dot_dot(root);
	size_t pos = 0;
	if ((pos = fullpath.find(root)) != 0)
	{
		return fullpath;
	}
	size_t len = root.size();
	if (root[len - 1] == '/')
	{
		return fullpath.substr(len);
	}
	else
	{
		return fullpath.substr(len + 1);
	}
}

std::string   replace_all(std::string   str, const   std::string&   old_value, const   std::string&   new_value)
{
	for (std::string::size_type pos(0); pos != std::string::npos; pos += new_value.length()) {
		if ((pos = str.find(old_value, pos)) != std::string::npos)
			str.replace(pos, old_value.length(), new_value);
		else   break;
	}
	return   str;
}
const char *  std_remove_dot_dot(const char * path)
{
	std::string path0 = string_remove_dot_dot(path);
	int len = path0.size() + 1;
	char * res = (char*)malloc(len * sizeof(char));
	strcpy_s(res, len, path0.c_str());
	return res;
}
const char * std_relative_path(const char * path1, const char * path2)
{
	std::string path0 = string_relative_path(path1, path2);
	int len = path0.size() + 1;
	char * res = (char*)malloc(len * sizeof(char));
	strcpy_s(res, len, path0.c_str());
	return res;
}

void std_split(const char*text, const char*spe, char*** result, int *size)
{
	std::vector<std::string> res;
	string_split(res, text, spe);
	int len = res.size();
	if (len == 0)
	{
		*result = 0;
		*size = 0;
	}
	else
	{
		*size = len;
		*result = (char **)malloc(len * sizeof(int));
		char ** ary = *result;
		for (int i = 0; i < len; i++)
		{
			int len2 = res[i].size();
			ary[i] = (char *)malloc((len2 + 1) * sizeof(char));
			strcpy_s(ary[i], len2 + 1, res[i].c_str());
		}
	}
}
const char * std_replay_all(const char * text, const char * str1, const char * str2)
{
	std::string  path0 = replace_all(text, str1, str2);
	int len = path0.size() + 1;
	char * res = (char*)malloc(len * sizeof(char));
	strcpy_s(res, len, path0.c_str());
	return res;
}
void std_combine_path(const char * path0, const char * path1, char *result, int size)
{
	std::string  path = string_combine_path(path0, path1);
	strcpy_s(result, size, path.c_str());
}
void std_relative_path_ex(const char * path1, const char * path2, char *result, int count)
{
	std::string path0 = string_relative_path(path1, path2);
	strcpy_s(result, count, path0.c_str());

}
char * std_str_clone(char * text)
{
	int size = strlen(text);
	char * p = (char*)malloc((size + 1) * sizeof(char));
	strcpy_s(p, size + 1, text);
	return p;
}