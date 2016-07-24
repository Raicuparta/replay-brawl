using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MatchData {
    const int Header = 600673; // sanity check for serialization
    int Round = 0; // current round number
    public List<Vector3> Steps;
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

    public byte[] ToBytes(List<Vector3> steps) {
        MemoryStream memStream = new MemoryStream();
        BinaryWriter w = new BinaryWriter(memStream);
        w.Write(Header);
        w.Write(Round);

        // Write player steps
        w.Write(steps.Count);
        foreach (Vector3 v in steps) {
            w.Write(v.x);
            w.Write(v.y);
            w.Write(v.z);
        }
        w.Close();
        byte[] buf = memStream.GetBuffer();
        memStream.Close();
        Debug.Log("Buffer size: " + buf.Length);
        return buf;
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

        // Get the number of steps
        int nSteps = r.ReadInt32();
        Debug.Log("Read nSteps: " + nSteps);
        Steps = new List<Vector3>();

        for (int i = 0; i < nSteps; i++) {
            float x = r.ReadSingle();
            float y = r.ReadSingle();
            float z = r.ReadSingle();
            Steps.Add(new Vector3(x, y, z));
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
