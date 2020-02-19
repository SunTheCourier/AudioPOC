using NAudio.Wave;
using System;
/// <summary>
/// Stream for looping playback
/// </summary>
public class LoopStream : WaveStream
{
    private readonly WaveStream sourceStream;
    private TimeSpan loopStart;
    private TimeSpan loopEnd;

    /// <summary>
    /// Creates a new Loop stream
    /// </summary>
    public LoopStream(WaveStream sourceStream, long loopStartSamples)
    {
        this.sourceStream = sourceStream;
        EnableLooping = true;
        loopStart = TimeSpan.FromMilliseconds(Math.Ceiling((double)loopStartSamples / sourceStream.WaveFormat.SampleRate * 1000));
        loopEnd = sourceStream.TotalTime;
    }

    public LoopStream(WaveStream sourceStream, long loopStartSamples, long loopEndSamples)
    {
        this.sourceStream = sourceStream;
        EnableLooping = true;
        loopStart = TimeSpan.FromMilliseconds(Math.Ceiling((double)loopStartSamples / sourceStream.WaveFormat.SampleRate * 1000));
        loopEnd = TimeSpan.FromMilliseconds(Math.Ceiling((double)loopEndSamples / sourceStream.WaveFormat.SampleRate * 1000));
    }

    public LoopStream(WaveStream sourceStream, TimeSpan loopStart)
    {
        this.sourceStream = sourceStream;
        EnableLooping = true;
        this.loopStart = loopStart;
        loopEnd = sourceStream.TotalTime;
    }

    public LoopStream(WaveStream sourceStream, TimeSpan loopStart, TimeSpan loopEnd)
    {
        this.sourceStream = sourceStream;
        EnableLooping = true;
        this.loopStart = loopStart;
        this.loopEnd = loopEnd;
    }

    /// <summary>
    /// Use this to turn looping on or off
    /// </summary>
    public bool EnableLooping { get; set; }

    /// <summary>
    /// Return source stream's wave format
    /// </summary>
    public override WaveFormat WaveFormat
    {
        get { return sourceStream.WaveFormat; }
    }

    /// <summary>
    /// LoopStream simply returns
    /// </summary>
    public override long Length
    {
        get { return sourceStream.Length; }
    }

    /// <summary>
    /// LoopStream simply passes on positioning to source stream
    /// </summary>
    public override long Position
    {
        get { return sourceStream.Position; }
        set { sourceStream.Position = value; }
    }

    public TimeSpan LoopStart
    {
        get { return loopStart; }
        set { loopStart = value; }
    }

    public TimeSpan LoopEnd
    {
        get { return loopEnd; }
        set { loopEnd = value; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0)
            {
                if (sourceStream.Position == 0 || !EnableLooping)
                {
                    // something wrong with the source stream
                    break;
                }
                // loop
                if (sourceStream.CurrentTime == loopEnd)
                    sourceStream.CurrentTime = loopStart;
            }
            totalBytesRead += bytesRead;
        }
        return totalBytesRead;
    }
}