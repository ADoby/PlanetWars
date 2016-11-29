using strange.extensions.signal.impl;

public class VoidSignal : Signal { };

public class IntSignal : Signal<int> { };

public class StringSignal : Signal<string> { };

/// <summary>
/// Basic signal for start
/// </summary>
public class StartSignal : Signal { }

/// <summary>
/// Basic signal for update, float=deltaTime
/// </summary>
public class UpdateSignal : Signal<float> { }

/// <summary>
/// Basic signal for localization, string=language
/// </summary>
public class LocalizeSignal : Signal<string> { }