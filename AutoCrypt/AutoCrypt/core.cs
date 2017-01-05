using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AutoCrypt {
    abstract class EncryptDecrypt {
        public virtual void EncryptData(string path) { }
        public virtual void DecryptData(string path) { }
    }

    class Encrypt : EncryptDecrypt {
        // encrypt specified file
        public override void EncryptData(string path) {
            // init classes
            Messages messages = new Messages();
            EncryptDecrypt decrypt = new Decrypt();
            Validation validation = new Validation();
            Method methods = new Method();

            // variables
            byte[] secretKey = null; byte[] IV = null; ;
            string elapsedTime, directory, fileName, filePath, lastItem;
            bool firstRun=true, error = false, lastRun = false;

            // new list
            List<string> fileList = new List<string>();

            // add files to list if directory, else add only the path itself
            if (validation.ValidateDirectory(path)) { fileList = new List<string>(methods.GetFiles(path)); } else { fileList.Add(path); }

            // get last item in fileList
            lastItem = fileList[(fileList.Count - 1)];

            // new stopwatch
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try {
                // encrypt all files in fileList
                foreach (string file in fileList) {
                    // set and reset variables
                    byte[] encryptedBytes = null, fileAsBytes = null, extensionAsBytes = null;
                    IEnumerable<byte> fullBytes = null, plainBytes = null;
                    string extension = null;

                    // if last iteration
                    if (file.Equals(lastItem)){ lastRun = true; }

                    // read file
                    fileAsBytes = File.ReadAllBytes(file);

                    // extension to be appended to file
                    extension = Path.GetExtension(file);

                    // append extension to file
                    extensionAsBytes = Encoding.ASCII.GetBytes(extension);
                    plainBytes = fileAsBytes.Concat(extensionAsBytes);
                    fileAsBytes = plainBytes.ToArray();

                    // create rijndael object
                    Rijndael rijndael = Rijndael.Create();

                    // generate only one key/IV for the current instance
                    if(firstRun) { secretKey=Encoding.ASCII.GetBytes(methods.generateKey(16)); IV=Encoding.ASCII.GetBytes(methods.generateKey(16)); firstRun =false; Console.WriteLine(Environment.NewLine); }

                    // set values
                    rijndael.Key = secretKey;
                    rijndael.Mode = CipherMode.CBC;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.IV = IV;

                    // open streams with data
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);

                    // progress notification
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    methods.ClearCurrentConsoleLine();
                    if (!lastRun) { messages.ProcessMessage(file); } else { messages.ProcessCompleteMessage(); }

                    // encrypt
                    cryptoStream.Write(fileAsBytes, 0, fileAsBytes.Length);
                    cryptoStream.Close();

                    // append IV to content
                    fullBytes = memoryStream.ToArray().Concat(IV).ToArray();
                    encryptedBytes = fullBytes.ToArray();
                    memoryStream.Close();

                    // write encrypted content to file
                    System.IO.File.WriteAllBytes(file, encryptedBytes);

                    // progress notification
                    Console.SetCursorPosition(0, Console.CursorTop+1);
                    methods.ClearCurrentConsoleLine();
                    if (!lastRun) { messages.EncryptProgressMessage(file); } else { messages.EncryptionCompleteMessage(); }

                    // set file extension to .rip
                    File.Move(file, Path.ChangeExtension(file, ".rip"));

                    // clear resources
                    rijndael.Dispose();

                }
            } catch (Exception E) {
                error = true;
                if (E is UnauthorizedAccessException) { messages.AuthMessage(); }
                else if (E is CryptographicException) { messages.CryptoError(); }
                else { messages.UnexpectedError(); }
            }

            // print if no error was encountered
            if (!error) { messages.FileToRipMessage(); }

            // stop stopwatch
            stopWatch.Stop();

            // print elapsed time
            TimeSpan ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            messages.TimeMessage(elapsedTime);

            // catch file/directory creation/write errors
            try {
                // create directory if it does not already exist then add file containing key
                directory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "autocryptKeyStorage";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
                fileName = DateTime.Now.ToString("HH_mm_ss") + "_keyKeeper.key";
                filePath = directory + "\\" + fileName;
                File.WriteAllText(Path.Combine(directory, fileName), methods.WarpKey(Encoding.ASCII.GetString(secretKey)));

                // print if no error was encountered
                if(!error) { messages.SuccessMessage(); }

                // print if key is not empty or null
                if(!Encoding.ASCII.GetString(secretKey).Equals("") || !Encoding.ASCII.GetString(secretKey).Equals(null)) {
                    // provide user with decryption key
                    messages.KeyMessage(methods.WarpKey(Encoding.ASCII.GetString(secretKey)), filePath);
                }

            } catch(Exception E){
                // if matrix is null or if unexpected error
                if (E is ArgumentNullException){ messages.NoKeyError(); } else{ messages.UnexpectedError(); }
            }

            // countdown until program will close
            methods.Countdown(30);

        }
    }

    class Decrypt : EncryptDecrypt {
        // decrypt specified file
        public override void DecryptData(string path) {
            // init classes
            Messages messages = new Messages();
            Input input = new Input();
            Validation validation = new Validation();
            Method methods = new Method();

            // variables
            string userInput=null, lastItem;
            byte[] IV=null;
            bool error = false, lastRun = false;

            // validate length of key
            messages.AccessMessage();
            userInput = input.ReadInput();
            while(!validation.ValidateKey(userInput)) { messages.KeyError(); userInput = input.ReadInput(); }
            Console.WriteLine(Environment.NewLine);

            // if userinput is a path read specified file as string and set userInput to content string
            if (File.Exists(userInput)) {
                // reuse key length check aswell as check if extension is .key
                while (!validation.ValidateKey(userInput) && Path.GetExtension(userInput).Equals(".key")) { messages.KeyError(); userInput = input.ReadInput(); }
                userInput = File.ReadAllText(userInput);
            }

            // new list
            List<string> fileList = new List<string>();

            // add files to list if directory, else add only the path itself
            if (validation.ValidateDirectory(path)) { fileList = new List<string>(methods.GetFiles(path)); } else { fileList.Add(path); }

            // get last item in fileList
            lastItem = fileList[(fileList.Count - 1)];

            // new stopwatch
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                // encrypt all files in fileList
                foreach (string file in fileList)
                {
                    // set and reset variables
                    byte[] decryptedBytes = null, encryptedBytes = null;
                    string extension = null;

                    // if last iteration
                    if (file.Equals(lastItem)) { lastRun = true; }

                    // read file as bytes
                    encryptedBytes = File.ReadAllBytes(file);

                    // get IV from file
                    IV = Encoding.ASCII.GetBytes(Encoding.UTF8.GetString(encryptedBytes).Substring(Encoding.UTF8.GetString(encryptedBytes).Length - 16));

                    // remove IV from filecontent
                    encryptedBytes = encryptedBytes.Take(encryptedBytes.Count()-16).ToArray();

                    // create rijndael object
                    Rijndael rijndael = Rijndael.Create();

                    // set values
                    rijndael.Key = Encoding.ASCII.GetBytes(methods.CorrectKey(userInput.Trim()));
                    rijndael.Mode = CipherMode.CBC;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.IV = IV;

                    // new streams
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);

                    // progress notification
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    methods.ClearCurrentConsoleLine();
                    if (!lastRun) { messages.ProcessMessage(file); } else { messages.ProcessCompleteMessage(); }

                    // decrypt
                    cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    cryptoStream.Close();

                    decryptedBytes = memoryStream.ToArray();
                    memoryStream.Close();

                    // obtain true file extension and remove from decrypted bytes
                    for (int i=decryptedBytes.Count()-1; i>=0; i--){
                        extension += Convert.ToChar(decryptedBytes[i]);
                        if (extension.Contains(".")) { break; }
                    }

                    // reverse extension to obtain actual extension
                    extension = new string(extension.Reverse().ToArray());

                    // remove true file extension from decrypted bytes
                    decryptedBytes = decryptedBytes.Take(decryptedBytes.Count()-extension.Length).ToArray();

                    // write decrypted content to file
                    System.IO.File.WriteAllBytes(file, decryptedBytes);

                    // progress notification
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                    methods.ClearCurrentConsoleLine();
                    if (!lastRun) { messages.DecryptProgressMessage(file); } else{ messages.DecryptionCompleteMessage(); }

                    // revert to true file extension
                    File.Move(file, Path.ChangeExtension(file, extension));

                    // clear resources
                    rijndael.Dispose();

                }
            }
            catch (Exception E) {
                error = true;
                if (E is UnauthorizedAccessException) { messages.AuthMessage(); }
                else if(E is CryptographicException) { messages.CryptoError(); }
                else { messages.UnexpectedError(); }
            }

            // print if no error was encountered
            if (!error) { messages.FileToOriginalMessage(); }

            // stop stopwatch
            stopWatch.Stop();

            // print elapsed time
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            messages.TimeMessage(elapsedTime);

            // print if no error was encountered
            if (!error) { messages.DecryptMessage(); }

            // countdown until program will close
            methods.Countdown(30);

        }
    }

    class Method {
        // generate random key
        public string generateKey(int length) {
            // characters that may be used
            const string availableChars = "A0B1C2D3E4F5G6H7I8J9KzLyMxNwOvPuQtRsSrTqUpVoWnXmYlZk0j1i2h3g4f5e6d7c8b9a";
            // new random
            Random random = new Random();
            // loop through all available characters for the size of length and find a random character and add to an array, 
            // then return the full as a string
            return new string(Enumerable.Repeat(availableChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // find all files under a specific directory including subdirectories
        public List<String> GetFiles(String directory) {
            // return all files as full paths within a list
            return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).ToList();
        }

        // countdown until program ends
        public void Countdown(int time) {
            // countdown until program will close
            for (int a = time; a >= 0; a--) {
                Console.Write("\rThe program will automatically close in: {0}", a);
                System.Threading.Thread.Sleep(1000);
                if(a==10) { ClearCurrentConsoleLine(); }
            }
        }

        // clear line in console
        public void ClearCurrentConsoleLine() {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        // warp key
        public string WarpKey(string key) {
            // variables
            string full,warpedkey;
            string[] array;

            // set full to key, key will always be 16 characters
            full = key;

            // remove all empty spaces
            full = string.Join("", full.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

            // add empty entry at specific places
            for(int i=3;i<17;i++) { if(i==3 || i==8 || i==12 || i==14 || i==17) { full = full.Insert(i, " "); } }

            // give array block content
            array = full.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);

            // set warpedKey
            warpedkey = array[2]+array[4]+array[1]+array[0]+array[3];

            return warpedkey;
        }

        // correct the key
        public string CorrectKey(string key) {
            // variables
            string full, correctkey;
            string[] array;

            // set full to key, key will always be 16 characters
            full = key;

            // remove all empty spaces
            full = string.Join("", full.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

            // add empty entry at specific places
            for (int i = 3; i < 19; i++) { if (i == 3 || i == 9 || i == 14 || i == 18 || i == 19) { full = full.Insert(i, " "); } }

            // give array block content
            array = full.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);

            // set correctKey
            correctkey = array[3] + array[2] + array[0] + array[4] + array[1];

            return correctkey;
        }

    }
}
