

using NUnit.Framework;

using UnityEngine;

public class SaveFileTests { 

    private SaveFile save;

    [SetUp]
    public void SetUp() {
        SaveFile.DeleteSave("unit_test_1"); // clear any old data to avoid side-effects

        save = new SaveFile("unit_test_1");
        save.SaveAll();
    }
    
    // [Test]
    // public void TestSetup() {
        
    // }
    
    [Test]
    public void SaveExistsTestTrue() {
        Assert.IsTrue(save.Exists());
        Assert.IsTrue(SaveFile.SaveExists(save.save_name), "SaveFile.SaveExists should match Save().Exists()");
    }
    
    [Test]
    public void SaveExistsTestFalse() {
        SaveFile does_not_exist = new SaveFile("qweretyuiopasdfghjkl");
        Assert.IsFalse(does_not_exist.Exists(), $"save {does_not_exist.save_name} should not exist");
        Assert.IsFalse(SaveFile.SaveExists(does_not_exist.save_name), "SaveFile.SaveExists should match Save().Exists()");
    }

    [Test]
    public void IdempotentSaveAll() {
        SaveFile.DeleteSave("unit_test_2"); // avoid side effects

        string json1 = save.AsDuckDict().Jsonify();
        SaveFile save2 = new SaveFile("unit_test_2");
        save2.LoadFromJson(json1);
        save2.SaveAll();
        string json2 = save2.AsDuckDict().Jsonify();

        Assert.AreEqual(json1, json2);
    }

    // [Test]
    // public void IdempotentSaveProgress() {
    //     SaveFile.DeleteSave("unit_test_2"); // avoid side effects

    //     string json1 = save.AsDuckDict().Jsonify();
    //     SaveFile save2 = new SaveFile("unit_test_2");
    //     save2.LoadFromJson(json1);
    //     save2.SaveProgress();
    //     string json2 = save2.AsDuckDict().Jsonify();

    //     Assert.AreEqual(json1, json2);
    // }

    // [Test]
    // public void IdempotentSaveSettings() {
    //     SaveFile.DeleteSave("unit_test_2"); // avoid side effects

    //     string json1 = save.AsDuckDict().Jsonify();
    //     SaveFile save2 = new SaveFile("unit_test_2");
    //     save2.LoadFromJson(json1);
    //     save2.SaveSettings();
    //     string json2 = save2.AsDuckDict().Jsonify();

    //     Assert.AreEqual(json1, json2);
    // }
}