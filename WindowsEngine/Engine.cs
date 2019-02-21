using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;


namespace Engine.Core
{
    /// <summary>
    /// Auto generated class by the installer [DO NOT EDIT]
    /// </summary>
#if DEBUG
    public class Engine : EngineFrame
#else
    internal class Engine : EngineFrame
#endif
    {

        public override string __PUBLIC => "87I9yY6meY3E0493";

private FileVersionTemplate__0 c_0; private uint c_0_s;



#if DEBUG
        public Engine()
#else
        internal Engine()
#endif
        {
            c_0 = new FileVersionTemplate__0(@"C:\Test\vmplayer.exe", @"15.0.0 build-10134415"){ Flags = (byte)0 }; c_0.CheckFailed = () => { Fail((ushort)1); }; Scoring.ScoringItem si_0 = new Scoring.ScoringItem() {ExpectedState = (uint)375221, NumPoints = (short)10, ID = (ushort)1}; si_0.SuccessStatus = () => { return c_0.CompletedMessage; }; si_0.FailureStatus = () => { return c_0.FailedMessage; };Expect((ushort)1,si_0); 

        }

        protected override async Task Tick()
        {
            if(c_0?.Enabled ?? false){ c_0_s = await c_0.GetCheckValue(); RegisterCheck((ushort)1|((uint)c_0.Flags << 16),c_0_s);}

        }

        //
    }

    //cst
internal sealed class FileVersionTemplate__0 : CheckTemplate
{
private readonly SafeString FilePath;
private readonly SafeString TargetVersion;
/// <summary>
/// A file version check template
/// </summary>
/// <param name="args">[0:FilePath],[1:FileVersion]</param>
internal FileVersionTemplate__0(params string[] args)
{
if (args.Length < 2)
Enabled = false;
FilePath = args[0];
TargetVersion = args[1];
}
/// <summary>
/// Get the check value
/// </summary>
/// <returns></returns>
internal override async Task<uint> GetCheckValue()
{
if (!File.Exists(FilePath))
{
return PrepareState("");
}
var version = await Task.FromResult<uint>(PrepareState(FileVersionInfo.GetVersionInfo(FilePath).FileVersion.ToString().CompareTo(TargetVersion) >= 0));
return version;
}
}

//cst
//NOTE: This file is sourced from ScoringEngine/InstallerCore/CheckTemplate.cs
//      Changes made outside of the source file will not be applied!
/// <summary>
/// A check template to be used to define a new system check
/// </summary>
internal abstract class CheckTemplate
{
#region REQUIRED
/// <summary>
/// Get the state of the check as an unsigned integer (4 byte solution, little endian)
/// </summary>
/// <returns>The state of the check</returns>
internal abstract Task<uint> GetCheckValue();
/// <summary>
/// Create the check
/// </summary>
/// <param name="args">Arguments to pass to the check</param>
internal protected CheckTemplate(params string[] args)
{
}
#endregion
#region OPTIONAL
/// <summary>
/// Speed at which this value updates
/// </summary>
internal virtual uint TickDelay => 1000;
/// <summary>
/// The message to display for offline scoring when the check is completed
/// </summary>
internal virtual SafeString CompletedMessage => "Check passed";
/// <summary>
/// The message to display for offline scoring when the check is failed
/// </summary>
internal virtual SafeString FailedMessage => "Check failed";
#endregion
#region PRIVATE
/// <summary>
/// Timer for internal ticking
/// </summary>
private System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();
/// <summary>
/// State of current check
/// </summary>
private uint CachedState = 0;
/// <summary>
/// Used to lock the state check to only allow one async state to run
/// </summary>
private bool STATE_LOCK;
internal ushort Flags;
#endregion
//Specifications (since i literally cant think rn)
//GetState() -> Offline? -> Score += (Failed? -score : State = expected ? score : 0)
//              Online?  -> NetComm(CID,STATEVALUE)
//ONLINE SPEC: CANNOT TICK MORE THAN 1 TIME PER SECOND, MUST GET BATCHED (bandwith limiting)
//ONLINE SPEC: Consolidate data. IE: Hash strings using a PJW Hash, everything else is a number at or below 4 bytes. All states are reported as unsigned integers.
//ONLINE SPEC: Batch sizes should be less than [1024] byte returns
//ONLINE SPEC: Online batch ticking should be equal to or slower than internal ticking. Ticks are only called when the value is being batched for sending
//OFFLINE SPEC: Min tickrate = 1ms, default = 1000ms, max = 300000ms
//OFFLINE SPEC: Still hash strings with PJW Hash
//OFFLINE SPEC: Must return a score
//Implementation Specifications
//Returns the value of the check to communicate between networks
//internal abstract uint GetOnlineState()
//Returns the score of the state
//internal abstract int GetOfflineState()
/// <summary>
/// Can this check be ticked at this current point in time
/// </summary>
/// <returns></returns>
internal bool CanTick()
{
if (STATE_LOCK || Failed)
return false;
//Already ticking
if(!Timer.IsRunning)
{
Timer.Start();
return true;
}
return Timer.ElapsedMilliseconds >= TickDelay;
}
/// <summary>
/// Return the state of this check
/// </summary>
/// <returns></returns>
internal async Task<uint> CheckState()
{
if(CanTick())
{
STATE_LOCK = true;
CachedState = await GetCheckValue();
STATE_LOCK = false;
Timer.Restart();
}
return CachedState;
}
/// <summary>
/// Call to force the check to tick nice time checkstate is requested
/// </summary>
internal void ForceTick()
{
Timer.Stop();
Timer.Reset();
}
/// <summary>
/// Convert a string into a state
/// </summary>
/// <param name="content">The string to convert</param>
/// <returns>A state version of a string</returns>
private uint PrepareString(string content) //PJW hash
{
uint hash = 0, high;
foreach(char s in content)
{
hash = (hash << 4) + (uint)s;
if ((high = hash & 0xF0000000) > 0)
hash ^= high >> 24;
hash &= ~high;
}
return hash;
}
/// <summary>
/// Prepare a state for replication
/// </summary>
/// <param name="o_state">The state to be prepared</param>
/// <returns>A UINT version of the state</returns>
internal uint PrepareState(object o_state)
{
if (o_state == null)
return 0u;
return PrepareString(o_state.ToString());
}
private bool __enabled__ = true;
/// <summary>
/// Is this check enabled for evaluation?
/// </summary>
internal bool Enabled
{
get
{
return Failed ? false : __enabled__;
}
set
{
__enabled__ = Failed ? false : value;
}
}
/// <summary>
/// Has this check been irreversably failed
/// </summary>
private bool __failed__ = false;
/// <summary>
/// Has this check been irreversably failed
/// </summary>
internal bool Failed
{
get
{
return __failed__;
}
set
{
if (__failed__ || !value)
return;
__failed__ = true;
CheckFailed?.Invoke();
}
}
/// <summary>
/// A scoring event
/// </summary>
internal delegate void ScoringEvent();
/// <summary>
/// Called when the check fails
/// </summary>
internal ScoringEvent CheckFailed;
/// <summary>
/// Flags for a check definition
/// </summary>
[Flags]
internal enum CheckDefFlags : byte
{
}
private bool HasFlag(CheckDefFlags flag)
{
return (Flags & (byte)flag) > 0;
}
}
internal sealed class SafeString
{
private static readonly byte[] __key__ = new byte[] /*?installer.key*/{ 0x00, 0xc5, 0x6c, 0xdd, 0x38, 0x8d, 0xa7, 0x02, 0x43, 0x92, 0x96, 0xae, 0x31, 0x99, 0x8f, 0x79 };
private byte[] data;
/// <summary>
/// Create a string from a safe string
/// </summary>
/// <param name="s"></param>
public static implicit operator string(SafeString s)
{
return D(s.data);
}
/// <summary>
/// Create a safe string from a normal string
/// </summary>
/// <param name="s"></param>
public static implicit operator SafeString(string s)
{
SafeString st = new SafeString
{
data = E(s)
};
return st;
}
private static byte[] E(string str)
{
if (str == null)
str = "";
byte[] encrypted;
byte[] IV;
using (Aes aesAlg = Aes.Create())
{
aesAlg.Key = __key__;
aesAlg.GenerateIV();
IV = aesAlg.IV;
aesAlg.Mode = CipherMode.CBC;
var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
// Create the streams used for encryption.
using (var msEncrypt = new MemoryStream())
{
using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
{
using (var swEncrypt = new StreamWriter(csEncrypt))
{
//Write all data to the stream.
swEncrypt.Write(str);
}
encrypted = msEncrypt.ToArray();
}
}
}
var combinedIvCt = new byte[IV.Length + encrypted.Length];
Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);
// Return the encrypted bytes from the memory stream.
return combinedIvCt;
}
private static string D(byte[] str)
{
// Declare the string used to hold
// the decrypted text.
string plaintext = null;
// Create an Aes object
// with the specified key and IV.
using (Aes aesAlg = Aes.Create())
{
aesAlg.Key = __key__;
byte[] IV = new byte[aesAlg.BlockSize / 8];
byte[] cipherText = new byte[str.Length - IV.Length];
Array.Copy(str, IV, IV.Length);
Array.Copy(str, IV.Length, cipherText, 0, cipherText.Length);
aesAlg.IV = IV;
aesAlg.Mode = CipherMode.CBC;
// Create a decrytor to perform the stream transform.
ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
// Create the streams used for decryption.
using (var msDecrypt = new MemoryStream(cipherText))
{
using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
{
using (var srDecrypt = new StreamReader(csDecrypt))
{
// Read the decrypted bytes from the decrypting stream
// and place them in a string.
plaintext = srDecrypt.ReadToEnd();
}
}
}
}
return plaintext;
}
}
internal static class Extensions
{
#region Process Async from https://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why/39872058#39872058
public static async Task<int> StartProcess(
string filename,
string arguments,
string workingDirectory = null,
int? timeout = null,
TextWriter outputTextWriter = null,
TextWriter errorTextWriter = null)
{
using (var process = new Process()
{
StartInfo = new ProcessStartInfo()
{
CreateNoWindow = true,
Arguments = arguments,
FileName = filename,
RedirectStandardOutput = outputTextWriter != null,
RedirectStandardError = errorTextWriter != null,
UseShellExecute = false,
WorkingDirectory = workingDirectory
}
})
{
process.Start();
var cancellationTokenSource = timeout.HasValue ?
new CancellationTokenSource(timeout.Value) :
new CancellationTokenSource();
var tasks = new List<Task>(3) { process.WaitForExitAsync(cancellationTokenSource.Token) };
if (outputTextWriter != null)
{
tasks.Add(ReadAsync(
x =>
{
process.OutputDataReceived += x;
process.BeginOutputReadLine();
},
x => process.OutputDataReceived -= x,
outputTextWriter,
cancellationTokenSource.Token));
}
if (errorTextWriter != null)
{
tasks.Add(ReadAsync(
x =>
{
process.ErrorDataReceived += x;
process.BeginErrorReadLine();
},
x => process.ErrorDataReceived -= x,
errorTextWriter,
cancellationTokenSource.Token));
}
await Task.WhenAll(tasks);
return process.ExitCode;
}
}
/// <summary>
/// Waits asynchronously for the process to exit.
/// </summary>
/// <param name="process">The process to wait for cancellation.</param>
/// <param name="cancellationToken">A cancellation token. If invoked, the task will return
/// immediately as cancelled.</param>
/// <returns>A Task representing waiting for the process to end.</returns>
public static Task WaitForExitAsync(
this Process process,
CancellationToken cancellationToken = default(CancellationToken))
{
process.EnableRaisingEvents = true;
var taskCompletionSource = new TaskCompletionSource<object>();
EventHandler handler = null;
handler = (sender, args) =>
{
process.Exited -= handler;
taskCompletionSource.TrySetResult(null);
};
process.Exited += handler;
if (cancellationToken != default(CancellationToken))
{
cancellationToken.Register(
() =>
{
process.Exited -= handler;
taskCompletionSource.TrySetCanceled();
});
}
return taskCompletionSource.Task;
}
/// <summary>
/// Reads the data from the specified data recieved event and writes it to the
/// <paramref name="textWriter"/>.
/// </summary>
/// <param name="addHandler">Adds the event handler.</param>
/// <param name="removeHandler">Removes the event handler.</param>
/// <param name="textWriter">The text writer.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public static Task ReadAsync(
this Action<DataReceivedEventHandler> addHandler,
Action<DataReceivedEventHandler> removeHandler,
TextWriter textWriter,
CancellationToken cancellationToken = default(CancellationToken))
{
var taskCompletionSource = new TaskCompletionSource<object>();
DataReceivedEventHandler handler = null;
handler = new DataReceivedEventHandler(
(sender, e) =>
{
if (e.Data == null)
{
removeHandler(handler);
taskCompletionSource.TrySetResult(null);
}
else
{
textWriter.WriteLine(e.Data);
}
});
addHandler(handler);
if (cancellationToken != default(CancellationToken))
{
cancellationToken.Register(
() =>
{
removeHandler(handler);
taskCompletionSource.TrySetCanceled();
});
}
return taskCompletionSource.Task;
}
#endregion
/// <summary>
/// Safely set bytes in a list
/// </summary>
/// <param name="RawData">The list to set bytes in</param>
/// <param name="index">The index of the bytes</param>
/// <param name="bytes">The byte array to write</param>
public static void SetBytes(this List<byte> RawData, int index, byte[] bytes)
{
while (index + bytes.Length >= RawData.Count)
RawData.Add(0x0);
for (int i = 0;
i < bytes.Length;
i++)
{
RawData[i + index] = bytes[i];
}
return;
}
/// <summary>
/// Safely get bytes from a list of bytes
/// </summary>
/// <param name="RawData">The list to read from</param>
/// <param name="index">The index to read at</param>
/// <param name="count">The amount of bytes to read</param>
/// <returns></returns>
public static byte[] GetBytes(this List<byte> RawData, int index, int count)
{
while (index + count > RawData.Count)
RawData.Add(0x0);
byte[] bytes = new byte[count];
for (int i = 0;
i < count;
i++)
{
bytes[i] = RawData[i + index];
}
return bytes;
}
/// <summary>
/// Read a null terminated string from a byte list
/// </summary>
/// <param name="RawData">The raw data to parse from</param>
/// <param name="index">The index of the string. Gets modified to be past the null character</param>
/// <returns></returns>
public static string ReadString(this List<byte> RawData, ref int index)
{
string result = "";
while (index < RawData.Count && RawData[index] != 0x0)
{
result += (char)RawData[index];
index++;
}
index++;
return result;
}
}


}
