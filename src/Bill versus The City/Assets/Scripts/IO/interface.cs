public interface ISaveProgress {
    // interface for objects that store data in the progress section of the save file
    public void StartNewGame();
    public void LoadProgress(DuckDict progress_data);
}