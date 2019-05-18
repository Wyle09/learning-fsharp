module DataProgramin

open System.IO
open System
open System.Collections.ObjectModel

let lines = 
    File.ReadAllLines(@"C:\My Projects\FSharp\Data\Loan payments data.csv")
    |> Array.distinct
    |> Array.map(fun s -> s.Split(','))

let header = lines |> Array.take 1 // Get headers.
let data = lines |> Array.skip 1 // Get all data except headers.

[<Measure>] type days

type LoanStatus = 
    | PaidOff of PaidOffTime : DateTime
    | Collection of PastDueDays : int<days>
    | Collection_PaidOff of PaidOffTime : DateTime * PastDueDays : int<days>

[<Measure>] type dollar

//let transformtoloanstatus status = 
//    match status with
//    | "paidoff" -> PaidOff
//    | "collection" -> Collection
//    | "collection_paidoff" -> Collection_PaidOff
//    | unknown -> failwith(sprintf "unrecongnized loan status: \"%s\"" unknown)

let transformToLoanStatus(status, paidOffTime, pastDueDays) = 
    match status with
    | "PAIDOFF" 
        -> PaidOff(DateTime.Parse(paidOffTime))
    | "COLLECTION" 
        -> Collection(Int32.Parse(pastDueDays) * 1<days>)
    | "COLLECTION_PAIDOFF" 
        -> Collection_PaidOff(DateTime.Parse(paidOffTime), (Int32.Parse(pastDueDays)) * 1<days>)
    | unknown 
        -> failwith(sprintf "unrecongnized loan status: \"%s\"" unknown)

type LoanPayment = 
    {
        LoanId : string;
        LoanStatus : LoanStatus;
        Principal : int<dollar>;
        Terms : string;
        EffectiveDate : string;
        DueDate : string;
        PaidOffTime : string;
        PastDueDate : string;
        Age : string;
        Education : string;
        Gender : string;
    }

let transformToLoanPaymentData(data : string array) = // function example.
    {
        LoanId = data.[0];
        LoanStatus = transformToLoanStatus(data.[1], data.[6], data.[7]);
        Principal = Int32.Parse(data.[2]) * 1<dollar>;
        Terms = data.[3];
        EffectiveDate = data.[4];
        DueDate = data.[5];
        PaidOffTime = data.[6];
        PastDueDate = data.[7];
        Age = data.[8];
        Education = data.[9];
        Gender = data.[10];
    }

let paymentData = // Call the "tranformToLoanPaymentData" function.
    data
    |> Array.map transformToLoanPaymentData

// Example below, will find the distinct values for "loanStatus", can do the same for the 
// other columns as well.
//let distinctLoanStatusValues = 
//    paymentData
//    |> Array.map(fun pd -> pd.LoanStatus)
//    |> Array.distinct

//let distinctPrincipalValues = 
//    paymentData
//    |> Array.map(fun pd -> pd.Principal)
//    |> Array.distinct