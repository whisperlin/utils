
#include<stdio.h>
#include<string.h>
#ifndef _WIN32
#ifndef strcpy_s
void strcpy_s(char *desc, int size, char *src)
{
	int len = strlen(src);
	if (len > size - 1)
	{
		len = size - 1;
	}
	strncpy(desc, src,len);
	desc[len] = '\0';
}
#endif

#endif // !_WIN32

