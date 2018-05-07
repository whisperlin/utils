#ifndef _INIFILE_C_H
#define _INIFILE_C_H

#ifdef __cplusplus  
extern "C" {              
#endif  
	int *	ini_load(const char *text);
	void	ini_release(int ** handle);
	int		ini_get_section_count(int * handle);
	void	ini_get_section_by_index(int * handle,int index ,char * temp,int count);
	int		ini_get_value_count(int *handle,const char *section);
	void	ini_get_key_by_index(int *handle, const char *section,int index,char *temp,int count);
	void	ini_get_value(int *handle, const char *section, const char *key, char *temp, int count);

#ifdef __cplusplus  
}
#endif 
#endif
