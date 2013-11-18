namespace Apocalypse.Sockets.Definitions
{
    public enum OpCodes
    {
        ListDirectory,
        GetFileOptions,
        GetFilePart,
        TouchFile,
        DropFile,
        DropDir,
        SetFilePart,
        RunAndWait,
        RunNoWait,
        CreateDir,
        Error,
        Success,
        NotFound
    }
}
