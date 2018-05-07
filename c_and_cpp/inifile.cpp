#ifndef _INIFILE_CPP
#define _INIFILE_CPP

#include "inifile.hpp"
#include "inifile.h"
#include <stdlib.h>
#include <stdio.h>
#include <ctype.h>
#include "strUtil.hpp"
#include<stdio.h> 
namespace inifile
{

int INI_BUF_SIZE = 2048;

IniFile::IniFile()
{
    flags_.push_back("#");
    flags_.push_back(";");
}

bool IniFile::parse(const string &content, string &key, string &value, char c/*= '='*/)
{
    int i = 0;
    int len = content.length();

    while (i < len && content[i] != c) {
        ++i;
    }

    if (i >= 0 && i < len) {
        key = string(content.c_str(), i);
        value = string(content.c_str() + i + 1, len - i - 1);
        trim(key);
        trim(value);
        return true;
    }

    return false;
}

int IniFile::getline(string &str, FILE *fp)
{
    int plen = 0;
    int buf_size = INI_BUF_SIZE * sizeof(char);

    char *buf = (char *) malloc(buf_size);
    char *pbuf = NULL;
    char *p = buf;

    if (buf == NULL) {
        fprintf(stderr, "no enough memory!exit!\n");
        exit(-1);
    }

    memset(buf, 0, buf_size);
    int total_size = buf_size;

    while (fgets(p, buf_size, fp) != NULL) {
        plen = strlen(p);

        if (plen > 0 && p[plen - 1] != '\n' && !feof(fp)) {

            total_size = strlen(buf) + buf_size;
            pbuf = (char *)realloc(buf, total_size);

            if (pbuf == NULL) {
                free(buf);
                fprintf(stderr, "no enough memory!exit!\n");
                exit(-1);
            }

            buf = pbuf;

            p = buf + strlen(buf);

            continue;
        } else {
            break;
        }
    }

    str = buf;

    free(buf);
    buf = NULL;
    return str.length();

}
int IniFile::loadFromString(const string &text)
{
	IniSection * section = new IniSection();
	sections_[""] = section;
	 
	std::vector<std::string> dst;
	string_split(dst, text, "\n\r");
	int len = dst.size();
	string comment;
	for (int i = 0; i < len; i++)
	{
		std::string &line = dst[i];
		trimright(line, '\n');
		trimright(line, '\r');
		trim(line);

		if (!isComment(line)) {

			string subline;
			string tmp = line;

			for (size_t i = 0; i < flags_.size(); ++i) {
				subline = line.substr(0, line.find(flags_[i]));
				line = subline;
			}

			comment += tmp.substr(line.length());
		}

		trim(line);

		if (line.length() <= 0) {
			continue;
		}

		if (line[0] == '[') {
			section = NULL;
			int index = line.find_first_of(']');
			if (index == -1) {
				fprintf(stderr, "没有找到匹配的]\n");
				return -1;
			}

			int len = index - 1;

			if (len <= 0) {
				fprintf(stderr, "段为空\n");
				continue;
			}

			string s(line, 1, len);

			if (getSection(s.c_str()) != NULL) {
				fprintf(stderr, "此段已存在:%s\n", s.c_str());
				return -1;
			}

			section = new IniSection();
			sections_[s] = section;

			section->name = s;
			section->comment = comment;
			comment = "";
		}
		else if (isComment(line)) {
			if (comment != "") {
				comment += delim + line;
			}
			else {
				comment = line;
			}
		}
		else {
			string key, value;

			if (parse(line, key, value)) {
				IniItem item;
				item.key = key;
				item.value = value;
				item.comment = comment;
				section->items.push_back(item);
			}
			else {
				fprintf(stderr, "解析参数失败[%s]\n", line.c_str());
			}

			comment = "";
		}
	}
	return 1;
}


int IniFile::getSectionCount()
{
	return sections_.size()-1;
}
std::string IniFile::getSectionByIndex(int i)
{
	i++;
	auto iter = sections_.begin();
	for (int j = 0; j < i; j++)
	{
		iter++;
	}
	return iter->first;
}
 

IniSection *IniFile::getSection(const string &section /*=""*/)
{
    iterator it = sections_.find(section);

    if (it != sections_.end()) {
        return it->second;
    }

    return NULL;
}

string IniFile::getStringValue(const string &section, const string &key, int &ret)
{
    string value, comment;

    ret = getValue(section, key, value, comment);

    return value;
}

int IniFile::getIntValue(const string &section, const string &key, int &ret)
{
    string value, comment;

    ret = getValue(section, key, value, comment);

    return atoi(value.c_str());
}

double IniFile::getDoubleValue(const string &section, const string &key, int &ret)
{
    string value, comment;

    ret = getValue(section, key, value, comment);

    return atof(value.c_str());

}

int IniFile::getValue(const string &section, const string &key, string &value)
{
    string comment;
    return getValue(section, key, value, comment);
}
int IniFile::getValue(const string &section, const string &key, string &value, string &comment)
{
    IniSection *sect = getSection(section);

    if (sect != NULL) {
        for (IniSection::iterator it = sect->begin(); it != sect->end(); ++it) {
            if (it->key == key) {
                value = it->value;
                comment = it->comment;
                return RET_OK;
            }
        }
    }

    return RET_ERR;
}
int IniFile::getValues(const string &section, const string &key, vector<string> &values)
{
    vector<string> comments;
    return getValues(section, key, values, comments);
}

int IniFile::getValueCount(const string &section)
{
	IniSection *sect = getSection(section);
	if(sect)
		return sect->items.size();
	return 0;
}
std::string IniFile::getValueKeyByIndex(const string &section,int i)
{
	string value, comment;
 
	int idx = 0;
	IniSection *sect = getSection(section);
	if (sect != NULL) {
		for (IniSection::iterator it = sect->begin(); it != sect->end(); ++it,++idx) {
			if (idx == i)
				return it->key;
		}
	}
	return "";
}
int IniFile::getValues(const string &section, const string &key,
                       vector<string> &values, vector<string> &comments)
{
    string value, comment;

    values.clear();
    comments.clear();

    IniSection *sect = getSection(section);

    if (sect != NULL) {
        for (IniSection::iterator it = sect->begin(); it != sect->end(); ++it) {
            if (it->key == key) {
                value = it->value;
                comment = it->comment;

                values.push_back(value);
                comments.push_back(comment);
            }
        }
    }

    return (values.size() ? RET_OK : RET_ERR);

}
bool IniFile::hasSection(const string &section)
{
    return (getSection(section) != NULL);

}

bool IniFile::hasKey(const string &section, const string &key)
{
    IniSection *sect = getSection(section);

    if (sect != NULL) {
        for (IniSection::iterator it = sect->begin(); it != sect->end(); ++it) {
            if (it->key == key) {
                return true;
            }
        }
    }

    return false;
}
int IniFile::getSectionComment(const string &section, string &comment)
{
    comment = "";
    IniSection *sect = getSection(section);

    if (sect != NULL) {
        comment = sect->comment;
        return RET_OK;
    }

    return RET_ERR;
}
int IniFile::setSectionComment(const string &section, const string &comment)
{
    IniSection *sect = getSection(section);

    if (sect != NULL) {
        sect->comment = comment;
        return RET_OK;
    }

    return RET_ERR;
}

int IniFile::setValue(const string &section, const string &key,
                      const string &value, const string &comment /*=""*/)
{
    IniSection *sect = getSection(section);

    string comt = comment;

    if (comt != "") {
        comt = flags_[0] + comt;
    }

    if (sect == NULL) {
        sect = new IniSection();

        if (sect == NULL) {
            fprintf(stderr, "no enough memory!\n");
            exit(-1);
        }

        sect->name = section;
        sections_[section] = sect;
    }

    for (IniSection::iterator it = sect->begin(); it != sect->end(); ++it) {
        if (it->key == key) {
            it->value = value;
            it->comment = comt;
            return RET_OK;
        }
    }

    //not found key
    IniItem item;
    item.key = key;
    item.value = value;
    item.comment = comt;

    sect->items.push_back(item);

    return RET_OK;

}
void IniFile::getCommentFlags(vector<string> &flags)
{
    flags = flags_;
}
void IniFile::setCommentFlags(const vector<string> &flags)
{
    flags_ = flags;
}
void IniFile::deleteSection(const string &section)
{
    IniSection *sect = getSection(section);

    if (sect != NULL) {

        sections_.erase(section);
        delete sect;
    }
}
void IniFile::deleteKey(const string &section, const string &key)
{
    IniSection *sect = getSection(section);

    if (sect != NULL) {
        for (IniSection::iterator it = sect->begin(); it != sect->end(); ++it) {
            if (it->key == key) {
                sect->items.erase(it);
                break;
            }
        }
    }

}

void IniFile::release()
{
    fname_ = "";

    for (iterator i = sections_.begin(); i != sections_.end(); ++i) {
        delete i->second;
    }

    sections_.clear();

}

bool IniFile::isComment(const string &str)
{
    bool ret = false;

    for (size_t i = 0; i < flags_.size(); ++i) {
        size_t k = 0;

        if (str.length() < flags_[i].length()) {
            continue;
        }

        for (k = 0; k < flags_[i].length(); ++k) {
            if (str[k] != flags_[i][k]) {
                break;
            }
        }

        if (k == flags_[i].length()) {
            ret = true;
            break;
        }
    }

    return ret;
}
//for debug
void IniFile::print()
{
    printf("filename:[%s]\n", fname_.c_str());

    printf("flags_:[");

    for (size_t i = 0; i < flags_.size(); ++i) {
        printf(" %s ", flags_[i].c_str());
    }

    printf("]\n");

    for (iterator it = sections_.begin(); it != sections_.end(); ++it) {
        printf("section:[%s]\n", it->first.c_str());
        printf("comment:[%s]\n", it->second->comment.c_str());

        for (IniSection::iterator i = it->second->items.begin(); i != it->second->items.end(); ++i) {
            printf("    comment:%s\n", i->comment.c_str());
            printf("    parm   :%s=%s\n", i->key.c_str(), i->value.c_str());
        }
    }
}

void IniFile::trimleft(string &str, char c/*=' '*/)
{
    //trim head

    int len = str.length();

    int i = 0;

    while (str[i] == c && str[i] != '\0') {
        i++;
    }

    if (i != 0) {
        str = string(str, i, len - i);
    }
}

void IniFile::trimright(string &str, char c/*=' '*/)
{
    //trim tail
    int i = 0;
    int len = str.length();


    for (i = len - 1; i >= 0; --i) {
        if (str[i] != c) {
            break;
        }
    }

    str = string(str, 0, i + 1);
}

void IniFile::trim(string &str)
{
    //trim head

    int len = str.length();

    int i = 0;

    while (isspace(str[i]) && str[i] != '\0') {
        i++;
    }

    if (i != 0) {
        str = string(str, i, len - i);
    }

    //trim tail
    len = str.length();

    for (i = len - 1; i >= 0; --i) {
        if (!isspace(str[i])) {
            break;
        }
    }

    str = string(str, 0, i + 1);
}

#ifdef __cplusplus  
extern "C" {
#endif 
int *	ini_load(const char *text)
{
	inifile::IniFile *ini = new inifile::IniFile();
	ini->loadFromString(text);
	return (int*)ini;
}
void	ini_release(int ** handle)
{
	if (*handle)
	{
		inifile::IniFile *ini = (inifile::IniFile*) *handle;
		delete ini;
		*handle = 0;
	}
	
}
int		ini_get_section_count(int * handle)
{
	if (!handle)
	{
		return 0;
	}
	inifile::IniFile *ini = (inifile::IniFile*) handle;
	return ini->getSectionCount();
}
void	ini_get_section_by_index(int * handle, int index, char * temp, int count)
{
	if (!handle)
	{
		temp[0] = '\0';
		return;
	}
	inifile::IniFile *ini = (inifile::IniFile*) handle;
	std::string s = ini->getSectionByIndex(index);
	strcpy_s(temp, count, s.c_str());
}
int		ini_get_value_count(int *handle, const char *section)
{
	if (!handle)
	{
		return 0;
	}
	inifile::IniFile *ini = (inifile::IniFile*) handle;
	return ini->getValueCount(section);

}
void ini_get_key_by_index(int *handle, const char *section, int index, char *temp, int count)
{
	if (!handle)
	{
		temp[0] = '\0';
		return ;
	}
	inifile::IniFile *ini = (inifile::IniFile*) handle;
	std::string key = ini->getValueKeyByIndex(section, index);
	strcpy_s(temp, count, key.c_str());
}
void	ini_get_value(int *handle, const char *section, const char *key, char *temp, int count)
{
	if (!handle)
	{
		temp[0] = '\0';
		return;
	}
	inifile::IniFile *ini = (inifile::IniFile*) handle;
	std::string v;
	ini->getValue(section, key, v);
	strcpy_s(temp, count, v.c_str());
}

#ifdef __cplusplus  
}
#endif 

}
#endif




/*
char section[255];
char key[255];
char value[255];
int * handle = ini_load("[GAME]\nenv = test\nversion = 0\n[SEC]asset_http_addr = web01 - s3.s3.ejoy.com\nasset_http_port = 19780");
int len = ini_get_section_count(handle);
for (int i = 0; i < len; i++)
{
ini_get_section_by_index(handle, i, section, 255);
printf("[%s]\n", section);

int len2 = ini_get_value_count(handle, section);
for (int j = 0; j < len2; j++)
{
ini_get_key_by_index(handle, section, j, key, 255);
ini_get_value(handle, section, key, value, 255);

printf("%s = %s\n", key, value);
}

}
ini_release(&handle);

inifile::IniFile ini;
ini.loadFromString("[GAME]\nenv = test\nversion = 0\n[SEC]asset_http_addr = web01 - s3.s3.ejoy.com\nasset_http_port = 19780");
int len = ini.getSectionCount();
for (int i = 0; i < len; i++)
{
std::string s = ini.getSectionByIndex(i);
printf("[%s]\n", s.c_str());
int len2 = ini.getValueCount(s);
for (int j = 0; j < len2; j++)
{
std::string key = ini.getValueKeyByIndex(s, j);
std::string value;
ini.getValue(s, key, value);
printf("%s = %s\n", key.c_str(), value.c_str());
}
}*/
