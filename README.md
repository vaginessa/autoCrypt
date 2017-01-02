The purpose of this program is to be able to encrypt and decrypt single files or all files below a single directory using rijndael.

The program is built as a console application in order to allow an automated process to be set up more easily. It may also be used as a standalone application for data protection with user interaction. 

Before file content is encrypted the original file's extension will be obtained and appended as bytes to the file, a decryption key and IV will be randomly generated for every instance in order to easily decrypt all files under a single directory. IV will be appended to the encrypted file.

Once the file is encrypted the file extension of this file will change to .rip in order to protect the original file type.
The program will automatically close after a maximum of 30 seconds after successful or not successful encryption or decryption.

After a minimum of one successfully encrypted file the program will provide a key onscreen upon completion or failiure aswell as saved to file under [main drive]\autocryptKeyStorage\\[timestamp]_keykeeper.key.

Keys are scrambled before used and may only be used in this program, attempting to decrypt the files with the key elsewhere will not be possible as the key needs to be corrected before usage.

In the event of unsuccessful encryption or decryption the program will cast an "unexpected error" then terminate the instance after a maximum of 30 seconds, note that all previously processed files will still be encrypted or decrypted. If unexpected error is cast during encryption a key will be provided as usual on screen aswell as saved to file.

# Advantages:
1. Encryption/decryption using Rijndael
2. Original file extension is not visible until decryption
3. Keys are automatically scrambled before usage
4. Keys are provided onscreen and to file
5. current file being processed and last encrypted/decrypted file is actively displayed on screen
6. Total time spent encrypting/decrypting is displayed after completion/failiure
7. Program allows single files or all files under a certain directory to be encrypted/decrypted
8. Keys may be provided to the program as plain key or path to valid .key file
9. User input validation and error management
10. Automatically terminate itself at a maximum of 30 seconds after successful or unsuccessful encryption/decryption
11. Files can be encrypted over and over
12. Encryption/decryption runs in a separate thread in order to keep the program from freezing

# Drawbacks:
1. Certain files that require higher permission to be tampered with will throw error and terminate the program
2. Certain files that may not be tampered with at all will throw error and terminate the program
3. Files with different keys or non-encrypted files that was not present during encryption will throw error and terminate the program when decrypted

# Example usage:
1. Automated process for data protection
2. Standalone program for data protection
