#ifndef _INIFILE_H
#define _INIFILE_H

#include <map>
#include <vector>
#include <string>
#include <string.h>

using namespace std;
namespace inifile
{
	const int RET_OK = 0;
	const int RET_ERR = -1;
	const string delim = "\n";
	struct IniItem {
		string key;
		string value;
		string comment;
	};
	struct IniSection {
		typedef vector<IniItem>::iterator iterator;
		iterator begin() {
			return items.begin();
		}
		iterator end() {
			return items.end();
		}

		string name;
		string comment;
		vector<IniItem> items;
	};

	class IniFile
	{
	public:
		IniFile();
		~IniFile() {
			release();
		}

	public:
		typedef map<string, IniSection *>::iterator iterator;

		iterator begin() {
			return sections_.begin();
		}
		iterator end() {
			return sections_.end();
		}
	public:

		int loadFromString(const string &text);
		int getSectionCount();
		std::string getSectionByIndex(int i);
		int getValueCount(const string &section);
		std::string getValueKeyByIndex(const string &section, int i);

		string getStringValue(const string &section, const string &key, int &ret);

		int getIntValue(const string &section, const string &key, int &ret);

		double getDoubleValue(const string &section, const string &key, int &ret);

		int getValue(const string &section, const string &key, string &value);

		int getValue(const string &section, const string &key, string &value, string &comment);


		int getValues(const string &section, const string &key, vector<string> &values);


		int getValues(const string &section, const string &key, vector<string> &value, vector<string> &comments);

		bool hasSection(const string &section);
		bool hasKey(const string &section, const string &key);


		int getSectionComment(const string &section, string &comment);

		int setSectionComment(const string &section, const string &comment);

		void getCommentFlags(vector<string> &flags);

		void setCommentFlags(const vector<string> &flags);


		int setValue(const string &section, const string &key, const string &value, const string &comment = "");

		void deleteSection(const string &section);

		void deleteKey(const string &section, const string &key);
	public:
		static void trimleft(string &str, char c = ' ');
		static void trimright(string &str, char c = ' ');
		static void trim(string &str);
	private:
		IniSection *getSection(const string &section = "");
		void release();
		int getline(string &str, FILE *fp);
		bool isComment(const string &str);
		bool parse(const string &content, string &key, string &value, char c = '=');
		//for dubug
		void print();

	private:
		map<string, IniSection *> sections_;
		string fname_;
		vector<string> flags_;
	};
}


#endif
