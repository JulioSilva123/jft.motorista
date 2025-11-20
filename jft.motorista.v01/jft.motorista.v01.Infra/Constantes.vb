Imports System.IO

Module Constantes
    Public Const DatabaseFilename As String = "jft.motorista.v01.db3"


    ' open the database in read/write mode
    ' create the database if it doesn't exist
    ' enable multi-threaded database access
    Public Const Flags As SQLite.SQLiteOpenFlags = SQLite.SQLiteOpenFlags.ReadWrite Or SQLite.SQLiteOpenFlags.Create Or SQLite.SQLiteOpenFlags.SharedCache

    Public ReadOnly Property DatabasePath As String
        Get
            Dim basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Return Path.Combine(basePath, DatabaseFilename)
        End Get
    End Property


End Module
