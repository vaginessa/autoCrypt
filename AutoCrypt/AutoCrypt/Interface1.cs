using System;
using System.IO;

namespace AutoCrypt {
    interface UserMessages {
        void InitialMessage();
        void OptionMessage();
        void SuccessMessage();
        void DecryptMessage();
        void KeyMessage(string key,string path);
        void AccessMessage();
        void AuthMessage();
        void ProcessMessage(string file);
        void EncryptProgressMessage(string file);
        void DecryptProgressMessage(string file);
        void FileToRipMessage();
        void FileToOriginalMessage();
        void TimeMessage(string elapsedTime);
        void ProcessCompleteMessage();
        void EncryptionCompleteMessage();
        void DecryptionCompleteMessage();

        void OptionError();
        void InitialError();
        void KeyError();
        void CryptoError();
        void NoKeyError();
        void FileCreationError(string key);
        void UnexpectedError();
    }

    interface UserInput {
        string ReadInput();
    }

    interface Validate {
        bool ValidateInput(string str);
        bool ValidatePath(string path);
        bool ValidateFile(string path);
        bool ValidateDirectory(string path);
        bool ValidateKey(string key);
    }

    // messages that will show in console
    class Messages : UserMessages {
        string newLine = Environment.NewLine;
        public void InitialMessage() { Console.Write("Enter the path to your file or folder"+newLine+"Path: "); }
        public void OptionMessage() { Console.Write("Would you like to encrypt or decrypt this file?[ENC/DEC]"+newLine+"Answer: "); }
        public void SuccessMessage() { Console.WriteLine(newLine+"Your file(s) has been encrypted."); }
        public void DecryptMessage() { Console.WriteLine(newLine+"Your file(s) has been decrypted using the specified key."+newLine); }
        public void KeyMessage(string key, string path) { Console.WriteLine("Your decryption key is: "+key+newLine+"Your key has been saved to file at: "+path+newLine); }
        public void AccessMessage() { Console.Write("Please provide the decryption key for your file."+newLine+"Key: "); }
        public void AuthMessage() { Console.WriteLine(newLine+newLine+"A file/directory you have insufficient rights to modify has been encountered."+newLine+"Any previous processed files has successfully completed."); }
        public void ProcessMessage(string file) { Console.Write("Processing: {0}", file); }
        public void EncryptProgressMessage(string file) { Console.Write("Encrypted : {0}", file); }
        public void DecryptProgressMessage(string file) { Console.Write("Decrypted : {0}", file); }
        public void FileToRipMessage() { Console.WriteLine("File extension has changed to .rip"); }
        public void FileToOriginalMessage() { Console.Write("File extension has reverted to original."+newLine); }
        public void TimeMessage(string elapsedTime) { Console.Write(newLine+"Time elapsed: "+elapsedTime+newLine+newLine); }
        public void ProcessCompleteMessage() { Console.Write("All files has been processed."); }
        public void EncryptionCompleteMessage() { Console.WriteLine("All files has been encrypted."); }
        public void DecryptionCompleteMessage() { Console.WriteLine("All files has been decrypted."); }


        public void OptionError() { Console.Write("Invalid answer, please use either ENC or DEC"+newLine+"Answer: "); }
        public void InitialError() { Console.Write("This path does not lead to valid file, please try again"+ newLine+"Path: "); }
        public void KeyError() { Console.Write("Invalid key, please provide a valid key/path"+newLine+"Key: "); }
        public void CryptoError() { Console.WriteLine(newLine+newLine+"Invalid key, you will not be permitted to retry in the current instance."+newLine+"Please restart the program if you wish to try again."); }
        public void NoKeyError() { Console.WriteLine("No action could be completed in the current instance, make sure you have sufficient rights for your files and try again."); }
        public void FileCreationError(string key) { Console.WriteLine("Directory/file could not be created in this instance, you decryption key is: "+key+newLine+"Please make sure you have sufficient rights to perform such action and try again."); }
        public void UnexpectedError() { Console.WriteLine(newLine+newLine+"An unexpected error was encountered, please close the program and try again."+newLine+"Only original files may reside in a directory,"+newLine+"files added after encryption will cause this error to occur when decrypting."+newLine+"Should the problem persist an alternative solution for the file in question may be required."); }
    }

    // fetch user input
    class Input : UserInput {
        public string ReadInput() { return Console.ReadLine(); }
    }

    // validate input
    class Validation : Validate {
        // validate selected option
        public bool ValidateInput(string str) { if (str.ToLower().Equals("enc") || str.ToLower().Equals("dec")) { return true; } else { return false; } }

        // check if valid path
        public bool ValidatePath(string path) { if (Directory.Exists(path) || File.Exists(path)) { return true; } else { return false; } }

        // check if file
        public bool ValidateFile(string path) { if (File.Exists(path)) { return true; } else { return false; } }

        // check if directory
        public bool ValidateDirectory(string path) { if (Directory.Exists(path)) { return true; } else { return false; } }

        // check length of key
        public bool ValidateKey(string key) { if (key.Length == 16 || File.Exists(key)) { return true; } else { return false; } }
    }
}
