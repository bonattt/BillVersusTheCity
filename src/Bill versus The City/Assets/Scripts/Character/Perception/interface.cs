

public interface IGlobalSoundsObserver
{
    // this interface allows a class to subscribe to recieve updates about all game-sounds, 
    // without a physical location or checking sound propegation.
    public void UpdateSound(ISound sound);
}