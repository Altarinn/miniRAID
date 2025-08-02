## Renderable
**Renderable: this state is visible to player in some sense. (i.e., indicators on screen)**
Implement `IRenderableState` !
```C#
public class XXX : BackendState, IRenderableState
{
    // ...
    
	// This will be called automatically after the state has been Registered.
	// "after" means the next time the Coroutine gets a MoveNext() return.
	public void ConstructRenderer()
	{
	    // Assign BackendState.renderer HERE
	    renderer = new XXXRenderer(); // Any renderer inherits StateRenderer  
	    UpdateRenderer(); // optional
	}

	// This will be called automatically after OnGlobalActionPostCast.
	// This need to be a light implementation as everyone calls it.
	public void UpdateRenderer()  
	{  
	    XXXRenderer r = (renderer as XXXRenderer);  
		
		// Do NULL check.
		// Renderer won't exist if you don't want it to be rendered.
		// But UpdateRenderer() is still possible to be called.
		if (r == null) { return; }
		
	    // ...

		// Remember to call this if renderer won't auto-refresh on new data
	    r.Refresh();
	}
}
```