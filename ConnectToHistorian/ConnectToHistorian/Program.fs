open System.Data.SqlClient
open System
open FSharp.Data
open System.Collections.Generic

[<Literal>]
let _query = 
    @"--Quering the WideHistory table in INSQL liked server
        DECLARE @tagNames NVARCHAR(MAX) = '',
	        @openQuery NVARCHAR(MAX),
	        @tsql NVARCHAR(MAX),
	        @linkedServer NVARCHAR(MAX) = 'INSQL';

            SELECT @tagNames += QUOTENAME(TagName, '[') + ','
            FROM TagRef
            WHERE TagName LIKE '%Mixer%';

            SET @tagNames = LEFT(@tagNames, LEN(@tagNames) - 1)
            SET @tsql = 'SELECT DateTime, ' + @tagNames + ' 
                        FROM WideHistory
                        WHERE DateTime > DATEADD(HH, -4, GETDATE())';
            SET @openQuery = 'SELECT * 
                            FROM OPENQUERY(' + @linkedServer + ','' ' + @tsql + ' '' ) 
                            ORDER BY DateTime ASC';

            EXECUTE(@openQuery);"

let _conn = new SqlConnection(@"Data Source=WIN-UIH9QHSEMDC;Initial Catalog=Runtime;Integrated Security=false;User Id=sa;Password=Password_1")

let queryToList() : List<string> =    
    let _lst = new List<string>()
    let _cmd = new SqlCommand(_query, _conn)
    
    try 
        try 
            _conn.Open()
            let _reader : SqlDataReader = _cmd.ExecuteReader()
            let _headers = 
                for _i in 0.. _reader.FieldCount do                     
                    _lst.Add(String.Concat(",", _reader.GetName.ToString()))

            let _lines = 
                while _reader.Read() do 
                    _lst.Add(_reader.GetValue(1).ToString())

            _conn.Close()
            _conn.Dispose()
            _lst
        
        with
            | :? SqlException as ex -> printf "Connection Failiure: %s" <| ex.Message |> Console.ReadLine |> ignore; _lst

    finally
        _conn.Close()
        _conn.Dispose()
        _lst |> ignore
        
    
let dataToCSV(_lst : List<string>) = 
    if _lst <> null then          
        let _valueLines = [for _row in _lst -> String.concat ",", _row.ToString]
        _valueLines 
        |> Seq.iter(fun _x -> printfn "%s" <| _x.ToString |> Console.ReadLine |> ignore


        //let _sw = new System.IO.StreamWriter(_filePath, true)
        //_valueLines |> List.map(string) |> _sw.Write

[<EntryPoint>]
let main argv = 
    let _filePath = String.Format("{0}{1}{2}{3}", "C:\\Users\\wyle.cordero\\Desktop\\", "Test", DateTime.Now.ToString("yyyyMMddHHmmssfff"), ".csv")
    let _lst = queryToList()
    let _data = dataToCSV(_lst)
    0