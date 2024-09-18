

public class DebugMode {

    private static DebugMode _inst;
    public static DebugMode inst {
        get {
            if (_inst == null) {
                _inst = new DebugMode(false);
            }
            return _inst;
        }
    }

    public bool debug_enabled;

    public DebugMode(bool initial_setting) {
        debug_enabled = initial_setting;
    }
}