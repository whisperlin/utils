#pragma once

#ifdef __cplusplus  
extern "C" {               // 告诉编译器下列代码要以C链接约定的模式进行链接  
#endif  
	const char * std_remove_dot_dot(const char * path);
	const char * std_relative_path(const char * path1, const char * path2);
	void std_split(const char*text, const char*spe, char *** result, int *size);
	const char * std_replay_all(const char * text, const char * str1, const char * str2);
#ifdef __cplusplus  
}
#endif 
