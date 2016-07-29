using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MatchData {
    const int Header = 600673; // sanity check for serialization
    int Round = 0; // current round number
    public List<Capture.Step> Steps;
    public bool HasWinner = false;
    // finish times for every round
    public List<float> FinishTimes;
    // true if host wins, false if opponent wins, for every round
    public List<bool> Victories;

    public MatchData() {
        Victories = new List<bool>();
        FinishTimes = new List<float>();
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
        Debug.Log("Buffer write to bytes");
        MemoryStream memStream = new MemoryStream();
        BinaryWriter w = new BinaryWriter(memStream);
        w.Write(Header);

        // Write the current round number
        w.Write(Round);

        int nVictories = Victories.Count;
        w.Write(nVictories);
        Debug.Log("Writing victories");
        foreach (bool victory in Victories) {
            Debug.Log("Writing victory");
            w.Write(victory);
        }
        
        int nTimes = FinishTimes.Count;
        w.Write(nTimes);
        Debug.Log("Writing times");
        foreach (float time in FinishTimes) {
            Debug.Log("Writing time");
            w.Write(time);
        }

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
        collect = 1,// 01
        attack = 2  // 10
    }

    void WriteStep(BinaryWriter w, Capture.Step step) {
        // We only save the necessary information, but we need a way to know what was stored.
        // So we use a bitmask. Each bit maps to an action, and we will only save info
        // for the actions that have their bit set to 1.
        int mask = GenerateMask(step);
        w.Write(mask);
        //Debug.Log("Writing mask to bytes: " + mask);
        //Debug.Log("Writing position to bytes: " + step.x + ", " + step.y);
        w.Write(step.x);
        w.Write(step.y);
        if (CheckMask(Mask.collect, mask)) w.Write(step.collect);
        //if (CheckMask(Mask.attack, mask)) w.Write(step.attack);
    }

    bool CheckMask(Mask action, int mask) {
        return ((int)action & mask) > 0;
    }

    int GenerateMask(Capture.Step step) {
        int mask = 0;
        if (step.collect != -1) mask |= (int)Mask.collect;
        if (step.attack) mask |= (int)Mask.attack;
        return mask;
    }

    Capture.Step ReadStep(BinaryReader r) {
        // Read the mask to know which values we can read from the bytes
        // The others are set to their default values
        int mask = r.ReadInt32();
        //Debug.Log("Read mask from bytes: " + mask);
        Capture.Step step = new Capture.Step();
        step.x = r.ReadSingle();
        step.y = r.ReadSingle();
        if (CheckMask(Mask.collect, mask)) step.collect = r.ReadInt32();
        else step.collect = -1; // -1 means undefined here
        // booelans default to false so we only need to set the attack to true if necessary
        if (CheckMask(Mask.attack, mask)) step.attack = true;

        //Debug.Log("Read position from bytes: " + step.x + ", " + step.y);
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

        int nVictories = r.ReadInt32();
        Debug.Log("Reading victories");
        for (int i = 0; i < nVictories; i++) {
            Debug.Log("Read victory");
            Victories.Add(r.ReadBoolean());
        }

        int nTimes = r.ReadInt32();
        Debug.Log("Reading times");
        for (int i = 0; i < nTimes; i++) {
            Debug.Log("Read time");
            FinishTimes.Add(r.ReadSingle());
        }

        // Read the number of steps
        int nSteps = r.ReadInt32();
        //Debug.Log("Read nSteps: " + nSteps);
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

    public void AddVictory(bool hostWins) {
        Victories.Add(hostWins);
        Debug.Log("Added victory: " + Victories.Count);
    }

    public void AddFinishTime(float time) {
        FinishTimes.Add(time);
        Debug.Log("Added time: " + FinishTimes.Count);
    }

    public class UnsupportedMatchFormatException : System.Exception {
        public UnsupportedMatchFormatException(string message) : base(message) { }
    }
}
