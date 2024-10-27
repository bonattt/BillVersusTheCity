using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestDialogueFile
{
    private DialogueFile f;
    [SetUp] 
    public void SetUp() {
        f = new DialogueFile("test");
    }

    private void AssertListsMatch(List<string> expected, List<string> result) {
        Assert.IsTrue(
            expected.SequenceEqual(result),
            $"lists should equal ({GetListDisplay(expected)} != {GetListDisplay(result)})"
        );
    }

    private string GetListDisplay(List<string> ls) {
        string str = "[";
        for(int i = 0; i < ls.Count; i++) {
            str += ls[i] + ", ";
        }
        str += "]";
        return str;
    }

    [Test]
    public void ParseLinesFromDataEmptyStringTest()
    {
        List<string> expected_lines = new List<string>();
        string test_data = "";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());

    }

    [Test]
    public void ParseLinesFromDataWhitespaceTest()
    {
        List<string> expected_lines = new List<string>();
        string test_data = "\t\n\t\n   \n";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());
    }

    [Test]
    public void ParseLinesFromDataOneLineTest()
    {
        List<string> expected_lines = new List<string>(){ "blocking bill enter left" };
        string test_data = "blocking bill enter left";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());
    }

    [Test]
    public void ParseLinesFromDataOneLineSemiColonTest()
    {        
        List<string> expected_lines = new List<string>(){ "blocking bill enter left" };
        string test_data = "blocking bill enter left;";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());
    }

    [Test]
    public void ParseLinesFromDataOneLineBlankNewlinesTest()
    {
        List<string> expected_lines = new List<string>(){ "blocking bill enter left" };
        string test_data = "\nblocking bill enter left\n";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());
    }

    [Test]
    public void ParseLinesFromDataNewlinesAndSemicolonsTest()
    {
        List<string> expected_lines = new List<string>(){ "blocking bill enter left" };
        string test_data = "\nblocking bill enter left;\n";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());
    }

    [Test]
    public void ParseLinesFromDataOopsJustSemicolonsTest()
    {
        List<string> expected_lines = new List<string>();
        string test_data = ";;;;;";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());
    }

    [Test]
    public void ParseLinesMultiLineTest()
    {
        List<string> expected_lines = new List<string>(){ "one", "two", "three", "four", "five", "six", "seven" };
        string test_data = "\none;two;three\nfour\n\t\tfive;\nsix;seven\n\n";
        f.ParseLinesFromData(test_data);
        AssertListsMatch(expected_lines, f.GetLines());
    }

    // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // // `yield return null;` to skip a frame.
    // [UnityTest]
    // public IEnumerator TestDialogueFileWithEnumeratorPasses()
    // {
    //     // Use the Assert class to test conditions.
    //     // Use yield to skip a frame.
    //     yield return null;
    // }

    
    [Test]
    public void IterationGetNextEmpty() {
        f.ParseLinesFromData("");
        f.ParseActions();
        Assert.IsNull(f.GetNextAction());
        Assert.IsNull(f.GetNextAction()); // call twice, should keep returning null
    }

    [Test]
    public void IterationResetIterator() {
        f.ParseLinesFromData("noop");
        f.ParseActions();
        IDialogueAction result = f.GetNextAction();
        Assert.IsNotNull(result, "precondition: first get should not be null");
        Assert.IsNull(f.GetNextAction(), "precondition: secnod get should be null"); 
        f.ResetIterator();
        IDialogueAction second_result = f.GetNextAction();
        Assert.AreSame(result, second_result, $"first item ({result}) should be returned after reset, got {second_result}");
    }

    [Test]
    public void IterationGetNextActionListEmpty() {
        f.ParseLinesFromData("");
        f.ParseActions();
        Assert.AreEqual(0, f.GetNextActionsList().Count, "empty list should be returned with no actions");
        Assert.AreEqual(0, f.GetNextActionsList().Count, "empty list should be returned with no actions");
        Assert.AreEqual(0, f.GetNextActionsList().Count, "empty list should be returned with no actions");
    }

    [Test]
    public void IterationGetNextActionList() {
        f.ParseLinesFromData("noop; say bill pingas; say bill pootis; enter dave left; noop; say dave PotUS");
        f.ParseActions();

        List<IDialogueAction> result1 = f.GetNextActionsList();
        List<IDialogueAction> result2 = f.GetNextActionsList();
        List<IDialogueAction> result3 = f.GetNextActionsList();
        List<IDialogueAction> result4 = f.GetNextActionsList();

        Assert.AreEqual(2, result1.Count, "action count");
        Assert.AreEqual(1, result2.Count, "action count");
        Assert.AreEqual(3, result3.Count, "action count");
        Assert.AreEqual(0, result4.Count, "action count");
    }

    [Test]
    public void NoopAction() {
        f.ParseLinesFromData("noop");
        f.ParseActions();
        IDialogueAction result = f.GetNextAction();
        
        Assert.IsFalse(result.wait_for_player_input, "wait_for_player_input");
    }

    [Test]
    public void NoopActionWithPause() {
        f.ParseLinesFromData("break");
        f.ParseActions();
        IDialogueAction result = f.GetNextAction();
        
        Assert.IsTrue(result.wait_for_player_input, "wait_for_player_input");
    }

    [Test]
    public void BlockingActionNoFacingStandingLeft() {
        f.ParseLinesFromData("enter bill left");
        f.ParseActions();
        DialogueMoveAction result = (DialogueMoveAction) f.GetNextAction();
        
        Assert.AreEqual("enter", result.cmd);
        Assert.AreEqual("bill", result.actor_name);
        Assert.AreEqual(StageDirection.left, result.side);
        Assert.AreEqual(StageDirection.right, result.facing);
        Assert.IsNull(result.pose, "pose should be null");
        Assert.IsFalse(result.wait_for_player_input, "wait_for_player_input");
    }

    [Test]
    public void BlockingActionNoFacingStandingRight() {
        f.ParseLinesFromData("enter bill right");
        f.ParseActions();
        DialogueMoveAction result = (DialogueMoveAction) f.GetNextAction();
        
        Assert.AreEqual("enter", result.cmd);
        Assert.AreEqual("bill", result.actor_name);
        Assert.AreEqual(StageDirection.right, result.side);
        Assert.AreEqual(StageDirection.left, result.facing);
        Assert.IsNull(result.pose, "pose should be null");
        Assert.IsFalse(result.wait_for_player_input, "wait_for_player_input");
    }

    [Test]
    public void BlockingActionWithFacing() {
        f.ParseLinesFromData("enter bill right facing right");
        f.ParseActions();
        DialogueMoveAction result = (DialogueMoveAction) f.GetNextAction();
        
        Assert.AreEqual("enter", result.cmd);
        Assert.AreEqual("bill", result.actor_name);
        Assert.AreEqual(StageDirection.right, result.side);
        Assert.AreEqual(StageDirection.right, result.facing);
        Assert.IsNull(result.pose, "pose should be null");
        Assert.IsFalse(result.wait_for_player_input, "wait_for_player_input");
    }

    [Test]
    public void BlockingActionWithPose() {
        f.ParseLinesFromData("enter bill right facing right angry");
        f.ParseActions();
        DialogueMoveAction result = (DialogueMoveAction) f.GetNextAction();
        
        Assert.AreEqual("enter", result.cmd);
        Assert.AreEqual("bill", result.actor_name);
        Assert.AreEqual(StageDirection.right, result.side);
        Assert.AreEqual(StageDirection.right, result.facing);
        Assert.AreEqual("angry", result.pose);
        Assert.IsFalse(result.wait_for_player_input, "wait_for_player_input");
    }

    [Test]
    public void SpeakAction() {
        f.ParseLinesFromData("say bill_protagonist pingas pootis PotUS 420.69");
        f.ParseActions();
        DialogueSpeachAction result = (DialogueSpeachAction) f.GetNextAction();
        
        Assert.AreEqual("say", result.cmd);
        Assert.AreEqual("bill_protagonist", result.speaker);
        Assert.AreEqual("pingas pootis PotUS 420.69", result.text);
        Assert.IsTrue(result.wait_for_player_input, "wait_for_player_input");
    }

    [Test]
    public void PoseAction() {
        f.ParseLinesFromData("pose bill angry");
        f.ParseActions();
        DialoguePoseAction result = (DialoguePoseAction) f.GetNextAction();
        
        Assert.AreEqual("pose", result.cmd);
        Assert.AreEqual("bill", result.actor_name);
        Assert.AreEqual("angry", result.pose);
        Assert.IsFalse(result.wait_for_player_input, "wait_for_player_input");
    }


    /// NEW TESTS: Blocking with pose, Blocking existing character preserves pose
}
