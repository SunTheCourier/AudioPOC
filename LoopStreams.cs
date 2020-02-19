using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
/// <summary>
/// Stream for looping playback
/// </summary>
public class LoopStreams : WaveStream
{
    private readonly WaveStream[] sourceStreams;
    private TimeSpan loopStart;
    private int arrayPosition;

    /// <summary>
    /// Creates a new Loop stream
    /// </summary>
    public LoopStreams(WaveStream[] sourceStream, long loopStartSamples, int defaultPos)
    {
        sourceStreams = sourceStream;
        EnableLooping = true;
        loopStart = TimeSpan.FromMilliseconds(Math.Ceiling((double)loopStartSamples / sourceStream.First().WaveFormat.SampleRate * 1000));
        arrayPosition = defaultPos;
    }

    /// <summary>
    /// Use this to turn looping on or off
    /// </summary>
    public bool EnableLooping { get; set; }

    /// <summary>
    /// LoopStream simply returns
    /// </summary>
    public override long Length
    {
        get { return sourceStreams.Length; }
    }

    public int ArrayPosition
    {
        get { return ArrayPosition; }
        set { arrayPosition = value; }
    }

    public int ArrayLength
    {
        get { return sourceStreams.Length; }
    }

    /// <summary>
    /// LoopStream simply passes on positioning to source stream
    /// </summary>
    public override long Position
    {
        get { return sourceStreams.First().Position; }
        set
        {
            for (int i = 0; i < sourceStreams.Length; i++)
                sourceStreams[i].Position = value;
        }
    }

    public TimeSpan LoopStart
    {
        get { return loopStart; }
        set { loopStart = value; }
    }

    public override WaveFormat WaveFormat
    {
        get { return sourceStreams.First().WaveFormat; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            int bytesRead = sourceStreams[arrayPosition].Read(buffer, offset + totalBytesRead, count - totalBytesRead);

            for (int i = 0; i < sourceStreams.Length; i++)
                if (i != arrayPosition)
                    sourceStreams[i].Seek(bytesRead, SeekOrigin.Current);

            if (bytesRead == 0)
            {
                if (sourceStreams[arrayPosition].Position == 0 || !EnableLooping)
                {
                    // something wrong with the source stream
                    break;
                }
                // loop
                for (int i = 0; i < sourceStreams.Length; i++)
                    sourceStreams[i].CurrentTime = loopStart;
            }
            totalBytesRead += bytesRead;
        }
        return totalBytesRead;
    }
}