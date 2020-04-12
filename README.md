FileGenerator - generates random file of specified length.
Format of generated file: lines of "{Number}. {Line}".

appsettings.json contains:
	- Encoding (supports only "Unicode" and "UTF-8")
	- OutputFolder - folder where generated file will be written. Creates output folder if it does not exist.
	
To run app execute command: "FileGenerator.exe {File size in bytes}"
Logs are written in logs folder.

TextFileSorter - sorts generated file.

appsettings.json contains:
	- Encoding (supports only "Unicode" and "UTF-8")
	- OutputFolder - folder where generated file will be written. Creates output folder if it does not exist.

To run app execute command: "TextFileSorter.exe {fileName to be sorted}"
Logs are written in logs folder.
