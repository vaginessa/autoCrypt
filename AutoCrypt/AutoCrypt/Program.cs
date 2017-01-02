using System.Threading;


namespace AutoCrypt {
    class Program {
        static void Main(string[] args) {
            // init class
            EncryptDecrypt encrypt = new Encrypt();
            EncryptDecrypt decrypt = new Decrypt();
            ValidateStart validate = new ValidateStart();
            Method methods = new Method();

            // create thread
            Thread thread;

            // origin input validation
            validate.ValidateOrigin();

            // if input is enc(encrypt) or dec(decrypt)
            if (validate.GetUserInput().ToLower().Equals("enc")) { thread = new Thread(()=>encrypt.EncryptData(validate.GetPath())); } else{ thread = new Thread(()=>decrypt.DecryptData(validate.GetPath())); }

            // start thread
            thread.Start();
        }
    }

    class Values {
        protected string userInput = null, path = null;
        public string GetUserInput() { return userInput; }
        public string GetPath() { return path; }
    }

    class ValidateStart: Values {
        // original output and user input
        public void ValidateOrigin() {
            // define classes
            Messages messages = new Messages();
            Input input = new Input();
            Validation validation = new Validation();

            // start program and validate
            messages.InitialMessage();
            userInput = input.ReadInput();
            while (!validation.ValidatePath(userInput)) { messages.InitialError(); userInput = input.ReadInput(); }

            // set path to userInput value
            path = userInput;

            // init user option message and validate
            messages.OptionMessage();
            userInput = input.ReadInput();
            while (!validation.ValidateInput(userInput)) { messages.OptionError(); userInput = input.ReadInput(); }
        }
    }
}
