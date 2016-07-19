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
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections.Generic;

public class PlayGui : BaseGui {
    public GameObject Playfield;
    public Capture Player;
    public Capture Opponent;

    private const string TESTSTRING = "4,0,0V0,0,0|4,-0.01569599,0V0,-0.7848,0|4,-0.047088,0V0,-1.5696,0|4,-0.09417599,0V0,-2.3544,0|4,-0.15696,0V0,-3.1392,0|4,-0.23544,0V0,-3.924,0|4,-0.329616,0V0,-4.7088,0|4,-0.439488,0V0,-5.4936,0|4,-0.565056,0V0,-6.2784,0|4,-0.70632,0V0,-7.0632,0|4,-0.6882211,0V0,0,0|4,-0.6789545,0V0,0,0|4,-0.67701,0V0,0,0|4,-0.6754544,0V0,0,0|4,-0.67421,0V0,0,0|4,-0.6732144,0V0,0,0|4,-0.6724179,0V0,0,0|4,-0.6717808,0V0,0,0|4,-0.671271,0V0,0,0|4,-0.6708633,0V0,0,0|4,-0.6705371,0V0,0,0|4,-0.670276,0V0,0,0|4,-0.6700673,0V0,0,0|4,-0.6699002,0V0,0,0|4,-0.6697666,0V0,0,0|4,-0.6696597,0V0,0,0|4,-0.6695742,0V0,0,0|4,-0.6695058,0V0,0,0|4,-0.6694511,0V0,0,0|4,-0.6694072,0V0,0,0|4,-0.6693722,0V0,0,0|4,-0.6693442,0V0,0,0|4,-0.6693218,0V0,0,0|4,-0.6693038,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4,-0.6692895,0V0,0,0|4.041722,-0.6692781,0V2.08608,0,0|4.093443,-0.6692689,0V2.58608,0,0|4.169165,-0.6692615,0V3.78608,0,0|4.260887,-0.6692556,0V4.58608,0,0|4.358608,-0.6692509,0V4.88608,0,0|4.46233,-0.6692472,0V5.18608,0,0|4.568051,-0.6692442,0V5.28608,0,0|4.705773,-0.6692418,0V6.88608,0,0|4.845494,-0.6692399,0V6.98608,0,0|4.989216,-0.6692383,0V7.18608,0,0|5.132937,-0.6692371,0V7.18608,0,0|5.276659,-0.6692361,0V7.18608,0,0|5.420381,-0.6692353,0V7.18608,0,0|5.564102,-0.6692347,0V7.18608,0,0|5.707824,-0.6692342,0V7.18608,0,0|5.851545,-0.6692338,0V7.18608,0,0|5.995267,-0.6692334,0V7.18608,0,0|6.118989,-0.6692332,0V6.18608,0,0|6.24271,-0.669233,0V6.18608,0,0|6.362432,-0.6692328,0V5.98608,0,0|6.472153,-0.6692327,0V5.48608,0,0|6.529875,-0.6692325,0V2.88608,0,0|6.551596,-0.6692325,0V1.08608,0,0|6.529875,-0.6692324,0V-0.68608,0,0|6.496153,-0.6692324,0V-1.68608,0,0|6.452432,-0.6692323,0V-2.18608,0,0|6.39871,-0.6692323,0V-2.68608,0,0|6.336988,-0.6692323,0V-3.08608,0,0|6.265267,-0.6692323,0V-3.58608,0,0|6.181545,-0.6692323,0V-4.18608,0,0|6.091824,-0.6692323,0V-4.48608,0,0|6.000102,-0.6692323,0V-4.58608,0,0|5.908381,-0.6692323,0V-4.58608,0,0|5.816659,-0.6692323,0V-4.58608,0,0|5.718937,-0.6692323,0V-4.88608,0,0|5.621216,-0.6692323,0V-4.88608,0,0|5.519494,-0.6692323,0V-5.08608,0,0|5.417772,-0.6692323,0V-5.08608,0,0|5.316051,-0.6692323,0V-5.08608,0,0|5.214329,-0.6692323,0V-5.08608,0,0|5.112607,-0.6692323,0V-5.08608,0,0|5.010885,-0.6692323,0V-5.08608,0,0|4.909163,-0.6692323,0V-5.08608,0,0|4.807442,-0.6692323,0V-5.08608,0,0|4.70572,-0.6692323,0V-5.08608,0,0|4.603998,-0.6692323,0V-5.08608,0,0|4.502276,-0.6692323,0V-5.08608,0,0|4.400555,-0.6692323,0V-5.08608,0,0|4.298833,-0.6692323,0V-5.08608,0,0|4.197111,-0.6692323,0V-5.08608,0,0|4.095389,-0.6692323,0V-5.08608,0,0|3.993668,-0.6692323,0V-5.08608,0,0|3.891946,-0.6692323,0V-5.08608,0,0|3.790225,-0.6692323,0V-5.08608,0,0|3.688503,-0.6692323,0V-5.08608,0,0|3.586782,-0.6692323,0V-5.08608,0,0|3.48506,-0.6692323,0V-5.08608,0,0|3.383339,-0.6692323,0V-5.08608,0,0|3.281617,-0.6692323,0V-5.08608,0,0|3.179896,-0.6692323,0V-5.08608,0,0|3.078174,-0.6692323,0V-5.08608,0,0|2.976453,-0.6692323,0V-5.08608,0,0|2.874731,-0.6692323,0V-5.08608,0,0|2.77301,-0.6692323,0V-5.08608,0,0|2.671288,-0.6692323,0V-5.08608,0,0|2.569566,-0.6692323,0V-5.08608,0,0|2.467845,-0.6692323,0V-5.08608,0,0|2.366123,-0.6692323,0V-5.08608,0,0|2.264402,-0.6692323,0V-5.08608,0,0|2.16268,-0.6692323,0V-5.08608,0,0|2.060959,-0.6692323,0V-5.08608,0,0|1.959237,-0.6692323,0V-5.08608,0,0|1.863516,-0.6692323,0V-4.78608,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.6692323,0V0,0,0|1.863516,-0.3649283,0V0,15.2152,0|1.863516,-0.07632032,0V0,14.4304,0|1.863516,0.1965917,0V0,13.6456,0|1.863516,0.4538077,0V0,12.8608,0|1.863516,0.6953277,0V0,12.076,0|1.965546,0.5794359,0V0,1.136868E-13,0|1.992292,0.544941,0V0,-0.7847996,0|1.992292,0.513549,0V0,-1.5696,0|1.992292,0.466461,0V0,-2.3544,0|1.992292,0.403677,0V0,-3.1392,0|1.992292,0.325197,0V0,-3.924,0|1.992292,0.2310211,0V0,-4.7088,0|1.992292,0.1211491,0V0,-5.4936,0|1.992292,-0.004418939,0V0,-6.2784,0|1.992292,-0.1456829,0V0,-7.0632,0|1.992292,-0.3026429,0V0,-7.848,0|1.992292,-0.4752989,0V0,-8.6328,0|1.992292,-0.6636509,0V0,-9.4176,0|1.992292,-0.8676988,0V0,-10.2024,0|1.992292,-0.7708471,0V0,0,0|1.992292,-0.7212589,0V0,0,0|1.992292,-0.6958698,0V0,0,0|1.992292,-0.6828706,0V0,0,0|1.992292,-0.6762151,0V0,0,0|1.992292,-0.6748185,0V0,0,0|1.992292,-0.6737012,0V0,0,0|1.992292,-0.6728074,0V0,0,0|1.992292,-0.6720924,0V0,0,0|1.992292,-0.6715204,0V0,0,0|1.992292,-0.6710627,0V0,0,0|1.992292,-0.6706966,0V0,0,0|1.992292,-0.6704037,0V0,0,0|1.992292,-0.6701694,0V0,0,0|1.992292,-0.669982,0V0,0,0|1.992292,-0.669832,0V0,0,0|1.992292,-0.669712,0V0,0,0|1.992292,-0.669616,0V0,0,0|1.992292,-0.6695393,0V0,0,0|1.992292,-0.6694778,0V0,0,0|1.992292,-0.6694287,0V0,0,0|1.992292,-0.6693894,0V0,0,0|1.992292,-0.6693579,0V0,0,0|1.992292,-0.6693327,0V0,0,0|1.992292,-0.6693126,0V0,0,0|1.992292,-0.6693126,0V0,0,0|";
    private const string BlocksRootName = "BlocksRoot";
    private const string XBlockPrefabName = "XBlock";
    private const string OBlockPrefabName = "OBlock";
    private const string LargeMarkPrefix = "LargeMark";  // + "X00", "X01", "O00", etc

    WidgetConfig DoneCfg = new WidgetConfig(0.0f, -0.2f, 0.8f, 0.2f, 60, "Done");
    WidgetConfig OkButtonCfg = new WidgetConfig(0.0f, 0.4f, 0.4f, 0.2f, 60, "OK");

    private TurnBasedMatch mMatch = null;
    private MatchData mMatchData = null;
    private string mFinalMessage = null;
    private char mMyMark = '\0';

    // has a block been shot and is in flight?
    private bool mBlockInFlight = false;

    // the countdown to check if all blocks are at rest
    private float mRestCheckCountdown = 0.0f;
    private const float RestCheckInterval = 1.0f;

    // maximum time to wait for all blocks to come to rest
    private const float MaxTurnTime = 8.0f;
    private float mEndTurnCountdown = MaxTurnTime;

    // tile size (for calculating which large mark to attribute to each box)
    private const float TileSize = 1.3f;

    private bool mEndingTurn = false;

    // mass of the player's block (it's more massive than other blocks to
    // make for more interesting gameplay)
    private const float PlayerBlockMass = 10.0f;

    // countdown to hide instructions
    private bool mShowInstructions = false;

    private void Reset() {
        mMatch = null;
        mMatchData = null;
        mFinalMessage = null;
        mMyMark = '\0';
        mEndTurnCountdown = MaxTurnTime;
        mBlockInFlight = false;
        mEndingTurn = false;
        mShowInstructions = false;
        Util.MakeVisible(Playfield, false);
    }

    public void LaunchMatch(TurnBasedMatch match) {
        Reset();
        mMatch = match;
        MakeActive();

        if (mMatch == null) {
            throw new System.Exception("PlayGui can't be started without a match!");
        }
        try {
            // Note that mMatch.Data might be null (when we are starting a new match).
            // MatchData.MatchData() correctly deals with that and initializes a
            // brand-new match in that case.
            mMatchData = new MatchData(mMatch.Data);
        } catch (MatchData.UnsupportedMatchFormatException ex) {
            mFinalMessage = "Your game is out of date. Please update your game\n" +
                "in order to play this match.";
            Debug.LogWarning("Failed to parse board data: " + ex.Message);
            return;
        }

        // determine if I'm the 'X' or the 'O' player
        mMyMark = mMatchData.GetMyMark(match.SelfParticipantId);

        bool canPlay = (mMatch.Status == TurnBasedMatch.MatchStatus.Active &&
                mMatch.TurnStatus == TurnBasedMatch.MatchTurnStatus.MyTurn);

        if (canPlay) {
            mShowInstructions = true;
        } else {
            mFinalMessage = ExplainWhyICantPlay();
        }

        // if the match is in the completed state, acknowledge it
        if (mMatch.Status == TurnBasedMatch.MatchStatus.Complete) {
            PlayGamesPlatform.Instance.TurnBased.AcknowledgeFinished(mMatch,
                    (bool success) => {
                if (!success) {
                    Debug.LogError("Error acknowledging match finish.");
                }
            });
        }

        // set up the objects to show the match to the player
        SetupObjects(canPlay);
    }

    private string ExplainWhyICantPlay() {
        switch (mMatch.Status) {
            case TurnBasedMatch.MatchStatus.Active:
                break;
            case TurnBasedMatch.MatchStatus.Complete:
                return mMatchData.Winner == mMyMark ? "Match finished. YOU WIN!" :
                        "Match finished. YOU LOST!";
            case TurnBasedMatch.MatchStatus.Cancelled:
            case TurnBasedMatch.MatchStatus.Expired:
                return "This match was cancelled.";
            case TurnBasedMatch.MatchStatus.AutoMatching:
                return "This match is awaiting players.";
            default:
                return "This match can't continue due to an error.";
        }

        if (mMatch.TurnStatus != TurnBasedMatch.MatchTurnStatus.MyTurn) {
            return "It's not your turn yet!";
        }

        return "Error";
    }

    protected override void DoGUI() {
        if (mFinalMessage != null) {
            GuiLabel(CenterLabelCfg, mFinalMessage);
            if (GuiButton(OkButtonCfg)) {
                Reset();
                gameObject.GetComponent<MainMenuGui>().MakeActive();
            }
            return;
        }

        if (GuiButton(DoneCfg)) {
            SetStandBy("Sending...");
            EndTurn();
        }
    }

    private static GameObject Spawn(GameObject parent, string prefabName) {
        GameObject o = (GameObject) GameObject.Instantiate(Resources.Load(prefabName));
        if (parent != null) {
            o.transform.parent = parent.transform;
        }
        return o;
    }

    private static GameObject Spawn(GameObject parent, string prefabName, Vector3 position, Quaternion rotation) {
        GameObject o = (GameObject) GameObject.Instantiate(Resources.Load(prefabName), position, rotation);
        if (parent != null) {
            o.transform.parent = parent.transform;
        }
        return o;
    }

    private void SetupObjects(bool canPlay) {
        // show the play field
        Util.MakeVisible(Playfield, true);

        string replay = mMatchData.Replay;
        if (replay != null) {
            Opponent.ReadFromString(mMatchData.Replay);
            Opponent.Replaying = true;
        }
        else {
            Debug.Log("------------PUMP IT UP--------------");
            Opponent.ReadFromString(TESTSTRING);
            Opponent.Replaying = true;
            //Opponent.gameObject.SetActive(false);
        }

        // create the blocks
        /*foreach (MatchData.BlockDesc desc in mMatchData.BlockDescs) {
            Spawn(GetBlocksRoot(), desc.mark == MatchData.MarkX ?
                XBlockPrefabName : OBlockPrefabName, desc.position, desc.rotation);
        }*/

        // create the block the player is shooting, if applicable
        if (canPlay) {
            // the block prefabs get instantiated in the right
            // position for this, so we don't need to translate
            /*GameObject o = Spawn(GetBlocksRoot(), mMyMark == MatchData.MarkX ?
                    XBlockPrefabName : OBlockPrefabName);
            o.AddComponent<AimController>().SetFireDelegate(OnBlockFired);
            o.AddComponent<CollisionSfx>();
            o.GetComponent<Rigidbody>().mass = PlayerBlockMass;*/
        }
    }

    // notifies us that a block was fired (and is currently in flight)
    void OnBlockFired() {
        mBlockInFlight = true;
        mShowInstructions = false;
    }

    void Update() {
        if (mEndingTurn) {
            return;
        }

        mRestCheckCountdown -= Time.deltaTime;

        if (mBlockInFlight) {
            mEndTurnCountdown -= Time.deltaTime;

            // are all the blocks at rest?
            /*if (mRestCheckCountdown < 0) {
                if (AllBlocksAtRest() || mEndTurnCountdown < 0) {
                    // all blocks are at rest -- we are ready to take the turn
                    EndTurn();
                } else {
                    // check again momentarily
                    mRestCheckCountdown = RestCheckInterval;
                }
            }*/
        }
    }

    string DecideNextToPlay() {
        if (mMatch.AvailableAutomatchSlots > 0) {
            // hand over to an automatch player
            return null;
        } else {
            // hand over to our (only) opponent
            Participant opponent = Util.GetOpponent(mMatch);
            return opponent == null ? null : opponent.ParticipantId;
        }
    }

    void EndTurn() {
        mEndingTurn = true;

        // save current state of the blocks into our match data
        /*mMatchData.ClearBlockDescs();
        foreach (GameObject o in GetAllGameBlocks()) {
            char mark = o.tag == "MarkX" ? MatchData.MarkX : MatchData.MarkO;
            mMatchData.AddBlockDesc(mark, o.transform.position, o.transform.rotation);
        }*/

        // do we have a winner?
        if (mMatchData.HasWinner) {
            FinishMatch();
        } else {
            TakeTurn();
        }
    }

    string GetAdversaryParticipantId() {
        foreach (Participant p in mMatch.Participants) {
            if (!p.ParticipantId.Equals(mMatch.SelfParticipantId)) {
                return p.ParticipantId;
            }
        }
        Debug.LogError("Match has no adversary (bug)");
        return null;
    }

    void FinishMatch() {
        bool winnerIsMe = mMatchData.Winner == mMyMark;

        // define the match's outcome
        MatchOutcome outcome = new MatchOutcome();
        outcome.SetParticipantResult(mMatch.SelfParticipantId,
            winnerIsMe ? MatchOutcome.ParticipantResult.Win : MatchOutcome.ParticipantResult.Loss);
        outcome.SetParticipantResult(GetAdversaryParticipantId(),
            winnerIsMe ? MatchOutcome.ParticipantResult.Loss : MatchOutcome.ParticipantResult.Win);

        // finish the match
        SetStandBy("Sending...");
        PlayGamesPlatform.Instance.TurnBased.Finish(mMatch, mMatchData.ToBytes(),
                    outcome, (bool success) => {
            EndStandBy();
            mFinalMessage = success ? (winnerIsMe ? "YOU WON!" : "YOU LOST!") :
                "ERROR finishing match.";
        });
    }

    void TakeTurn() {
        SetStandBy("Sending...");
        mMatchData.Replay = Player.ToString();

        PlayGamesPlatform.Instance.TurnBased.TakeTurn(mMatch, mMatchData.ToBytes(),
                    DecideNextToPlay(), (bool success) => {
            EndStandBy();
            mFinalMessage = success ? "Done for now!" : "ERROR sending turn.";
        });
    }
}
