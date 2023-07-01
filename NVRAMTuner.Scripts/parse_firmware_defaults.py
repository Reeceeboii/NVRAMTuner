"""
This script downloads the NVRAM defaults for the asuswrt-merlin firmware from the master
branch of the Merlin repository on GitHub. It then processes the file, ignoring any redundant data
etc..., extracts the variable names and then links them to their descriptions (or nothing if no
description comment was present in the original .C file).

It then uses this data to generate a dummy C# file with a hashtable keyed on the variable names.
"""

from msilib.schema import File
import urllib.request as req
import re
import json

def main() -> None:
    """Main function"""

    url: str = "https://raw.githubusercontent.com/RMerl/asuswrt-merlin.ng/master/release/src/router/shared/defaults.c"
    fileContent: str  = get_raw_string_content(url)

    variableDict: dict = {}
    
    # used ChatGPT for these regexes and I feel absolutely zero shame; would rather the chatbot blow its brains out than me
    for line in fileContent.split('\n'):
        if line and "{" in line and "struct" not in line:
            line = re.sub("[\t]*", "", line)
            values = line.split(",")

            for it, value in enumerate(values):
                values[it] = value.strip()
            
            # attempt to extract the variable name
            try:
                variable = re.findall('\"(.+?)\"', values[0])[0]
                variableDict[variable] = {}
            except:
                pass

            # attempt to extract the variable's default value
            try:
                variableDict[variable]["default"] = values[1].replace('"', '')
            except:
                pass

            # attempt to extract the variable's description
            try:
                variableDict[variable]["description"] = ""
                description = re.findall('\/\*\s*(.*?)\s*\*\/', values[len(values) - 1])[0]
                variableDict[variable]["description"] = description
            except:
                pass

    with open("./firmware_variable_defaults.json", "w") as file_obj:
        file_obj.write(json.dumps(variableDict))

def get_raw_string_content(url: str) -> str:
    """Downloads the raw .C file, reads the response, returns its content decoded as UTF-8 text"""
    response = req.urlopen(url)
    reponseData = response.read()
    return reponseData.decode('utf-8')

if __name__ == "__main__":
    main()
