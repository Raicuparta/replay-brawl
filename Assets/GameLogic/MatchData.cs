/*
 * Copyright (C) 2014 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MatchData {
    private const int Header = 600673; // sanity check for serialization

    public const char MarkNone = ' ';
    public const char MarkO = 'o';
    public const char MarkX = 'x';
    public const char MarkConflict = '!';

    public const int BoardSize = 3;
    private char[][] mBoard = new char[][] {
        new char[] { MarkNone, MarkNone, MarkNone },
        new char[] { MarkNone, MarkNone, MarkNone },
        new char[] { MarkNone, MarkNone, MarkNone }
    };

    public List<Vector3> Steps;

    private List<BlockDesc> mBlockDescs = new List<BlockDesc>();

    private bool mXWins = false;
    private bool mOWins = false;

    // the participant ID that plays as 'X' (the other one plays as 'O')
    private string mParticipantIdX = "";

    public MatchData() {

    }

    public MatchData(byte[] b) : this() {
        if (b != null) {
            ReadFromBytes(b);
            ComputeWinner();
        }
    }

    public char GetMark(int x, int y) {
        return (x >= 0 && x < mBoard.Length && y >= 0 && y < mBoard.Length) ?
            mBoard[x][y] : MarkNone;
    }

    public void SetMark(int x, int y, char mark) {
        if (x >= 0 && x < mBoard.Length && y >= 0 && y < mBoard.Length) {
            mBoard[x][y] = mark;
        }
        ComputeWinner();
    }

    public void ResetMarks() {
        int x, y;
        for (x = 0; x < mBoard.Length; x++) {
            for (y = 0; y < mBoard.Length; y++) {
                mBoard[x][y] = MarkNone;
            }
        }
    }

    public void ClearBlockDescs() {
        mBlockDescs.Clear();
    }

    public void AddBlockDesc(char mark, Vector3 position, Quaternion orientation) {
        mBlockDescs.Add(new BlockDesc(mark, position, orientation));
    }

    public List<BlockDesc> BlockDescs {
        get {
            return mBlockDescs;
        }
    }

    private static bool AllEqual(char[] arr) {
        foreach (char c in arr) {
            if (c != arr[0]) {
                return false;
            }
        }
        return true;
    }

    public char Winner {
        get {
            return (mOWins && mXWins) ? MarkConflict :
                mOWins ? MarkO : mXWins ? MarkX : MarkNone;
        }
    }

    public bool HasWinner {
        get {
            return (mXWins && !mOWins) || (!mXWins && mOWins);
        }
    }

    private void AddWinner(char mark) {
        if (MarkO == mark) {
            mOWins = true;
        } else if (MarkX == mark) {
            mXWins = true;
        }
    }

    private void ComputeWinner() {
        int x, y;
        char[] a = new char[mBoard.Length];

        mXWins = mOWins = false;

        // check columns
        for (x = 0; x < mBoard.Length; x++) {
            for (y = 0; y < mBoard.Length; y++) {
                a[y] = mBoard[x][y];
            }
            if (AllEqual(a)) {
                AddWinner(a[0]);
            }
        }

        // check rows
        for (y = 0; y < mBoard.Length; y++) {
            for (x = 0; x < mBoard.Length; x++) {
                a[x] = mBoard[x][y];
            }
            if (AllEqual(a)) {
                AddWinner(a[0]);
            }
        }

        // check diagonals
        for (x = 0; x < mBoard.Length; x++) {
              a[x] = mBoard[x][x];
            if (AllEqual(a)) {
                AddWinner(a[0]);
            }
        }
        for (x = 0; x < mBoard.Length; x++) {
            a[x] = mBoard[x][mBoard.Length - 1 - x];
            if (AllEqual(a)) {
                AddWinner(a[0]);
            }
        }
    }

    public byte[] ToBytes(List<Vector3> steps) {
        MemoryStream memStream = new MemoryStream();
        BinaryWriter w = new BinaryWriter(memStream);
        w.Write(Header);
        w.Write((byte)mParticipantIdX.Length);
        w.Write(mParticipantIdX.ToCharArray());


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

        int len = (int)r.ReadByte();
        mParticipantIdX = new string(r.ReadChars(len));

        // Get the number of steps
        int nSteps = (int)r.ReadInt32();
        Debug.Log("Read nSteps: " + nSteps);
        Steps = new List<Vector3>();

        for (int i = 0; i < nSteps; i++) {
            float x = r.ReadSingle();
            float y = r.ReadSingle();
            float z = r.ReadSingle();
            Steps.Add(new Vector3(x, y, z));
        }
    }

    public char GetMyMark(string myParticipantId) {
        if (mParticipantIdX.Equals("")) {
            // if X is unclaimed, claim it!
            mParticipantIdX = myParticipantId;
        }
        return mParticipantIdX.Equals(myParticipantId) ? MarkX : MarkO;
    }

    public struct BlockDesc {
        public char mark;
        public Vector3 position;
        public Quaternion rotation;
        public BlockDesc(char mark, Vector3 position, Quaternion rotation) {
            this.mark = mark;
            this.position = position;
            this.rotation = rotation;
        }
        public override string ToString () {
            return "[BlockDesc: '" + mark + "', pos=" + position + ", rot=" + rotation + "]";
        }
    };

    public class UnsupportedMatchFormatException : System.Exception {
        public UnsupportedMatchFormatException(string message) : base(message) {}
    }
}
