package com.company.assembleegameclient.util {
import flash.filesystem.FileStream;
import flash.filesystem.File;
import flash.filesystem.FileMode;

public class FileManager
{
    public var text:String;
    public var fileName:String;
    public var fileAppendix:String;

    public static function writeToFile(Text:String, FileName:String, FileAppendix:String) : void
    {
        //create an object that holds your data, this will act the same as the 'data' value of a shared object
        var saveString:String = Text;
        //using the File and FileStream classes to read/save files
        var file:File = File.applicationStorageDirectory.resolvePath(FileName + "." + FileAppendix); //or where and whatever you want to store and call the save file
        var fileStream:FileStream = new FileStream();
        fileStream.open(file, FileMode.WRITE);
        fileStream.writeUTF(saveString); //write the object to this file
        fileStream.close(); //close the File Stream
    }

    public static function readFromFile(FileName:String, FileAppendix:String) : String
    {
        var file:File = File.applicationStorageDirectory.resolvePath(FileName + "." + FileAppendix);

        if (!file.exists) {
            trace("File not found.");
            return null;
        }

        var fileStream:FileStream = new FileStream();
        fileStream.open(file, FileMode.READ);

        if (fileStream.bytesAvailable == 0) {
            trace("File is empty.");
            fileStream.close();
            return null;
        }

        var savedString:String;
        try {
            savedString = fileStream.readUTF();
        } catch (e:Error) {
            trace("Error reading file:", e);
            savedString = null;
        }

        fileStream.close();
        return(savedString);
    }

}
}
