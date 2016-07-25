using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MatchData {
    const int Header = 600673; // sanity check for serialization
    int Round = 0; // current round number
    public List<Capture.Step> Steps;
    public bool HasWinner = false;

    public MatchData() {

    }

    public MatchData(byte[] b) : this() {
        if (b != null) {
            ReadFromBytes(b);
            ComputeWinner();
        }
    }

    private void ComputeWinner() {
        // TODO
    }

    public byte[] ToBytes(List<Capture.Step> steps) {
        MemoryStream memStream = new MemoryStream();
        BinaryWriter w = new BinaryWriter(memStream);
        w.Write(Header);

        // Write the current round number
        w.Write(Round);

        // Write player steps
        // First we write the number of steps, so we know when to stop reading later
        w.Write(steps.Count);
        // Now we write each step
        foreach (Capture.Step step in steps) {
            WriteStep(w, step);
        }

        // Finish writing
        w.Close();
        byte[] buf = memStream.GetBuffer();
        memStream.Close();
        Debug.Log("Buffer size: " + buf.Length);
        return buf;
    }

    // Step masks
    enum Mask {
        x = 1,      // 0001
        y = 2,      // 0010
        collect = 4,// 0100
        attack = 8  // 1000
    }

    void WriteStep(BinaryWriter w, Capture.Step step) {
        // We only save the necessary information, but we need a way to know what was stored.
        // So we use a bitmask. Each bit maps to an action, and we will only save info
        // for the actions that have their bit set to 1.
        int mask = GenerateMask(step);
        w.Write(mask);
        Debug.Log("Writing mask to bytes: " + mask);
        Debug.Log("Writing position to bytes: " + step.x + ", " + step.y);
        if (CheckMask(Mask.x, mask)) w.Write(step.x);
        if (CheckMask(Mask.y, mask)) w.Write(step.y);
        if (CheckMask(Mask.collect, mask)) w.Write(step.collect);
        if (CheckMask(Mask.attack, mask)) w.Write(step.attack);
    }

    bool CheckMask(Mask action, int mask) {
        return ((int)action & mask) > 0;
    }

    int GenerateMask(Capture.Step step) {
        int mask = 0;
        if (step.x != 0) mask |= (int) Mask.x;
        if (step.y != 0) mask |= (int) Mask.y;
        if (step.collect != -1) mask |= (int)Mask.collect;
        if (step.attack) mask |= (int)Mask.attack;
        return mask;
    }

    Capture.Step ReadStep(BinaryReader r) {
        // Read the mask to know which values we can read from the bytes
        // The others are set to their default values
        int mask = r.ReadInt32();
        Debug.Log("Read mask from bytes: " + mask);
        Capture.Step step = new Capture.Step();
        if (CheckMask(Mask.x, mask)) step.x = r.ReadSingle();
        if (CheckMask(Mask.y, mask)) step.y = r.ReadSingle();
        if (CheckMask(Mask.collect, mask)) step.collect = r.ReadInt32();
        else step.collect = -1; // -1 means undefined here
        // booelans default to false so we only need to set the attack to true if necessary
        if (CheckMask(Mask.attack, mask)) step.attack = true;

        Debug.Log("Read position from bytes: " + step.x + ", " + step.y);
        return step;
    }

    private void ReadFromBytes(byte[] b) {
        BinaryReader r = new BinaryReader(new MemoryStream(b));
        int header = r.ReadInt32();
        if (header != Header) {
            // we don't know how to parse this version; user has to upgrade game
            throw new UnsupportedMatchFormatException("Board data header " + header +
                    " not recognized.");
        }
        // Read the current round number
        Round = r.ReadInt32();

        // Read the number of steps
        int nSteps = r.ReadInt32();
        Debug.Log("Read nSteps: " + nSteps);
        Steps = new List<Capture.Step>();
        for (int i = 0; i < nSteps; i++) {
            Steps.Add(ReadStep(r));
        }
    }

    public int GetRound() {
        return Round;
    }

    public void IncRound() {
        Round++;
    }

    public class UnsupportedMatchFormatException : System.Exception {
        public UnsupportedMatchFormatException(string message) : base(message) { }
    }
}
