
# Whats new in Unity ARKit Plugin for ARKit 2.0 


## ARWorldMap  

This allows you to keep track of places you have been during your session.  You can use it to relocalize yourself to a previous session map that you might have saved, or to send the map across to another device so that that device can relocalize itself to your space.

How to use it in Unity: see the _Examples/ARKit2.0/UnityARWorldMap/UnityARWorldMap.unity_ for an example

Every session builds up a **ARWorldMap** as you move around and detect more feature points.  To get the current **ARWorldMap** of a session, you have to use an asynchronous method:

```CS
       session.GetCurrentWorldMapAsync(OnWorldMap);       
```

And then on the callback, you will get an **ARWorldMap**, which is the current world map.  You can keep it in memory somewhere, send it to someone else or save it:

```CS
    void OnWorldMap(ARWorldMap worldMap)
    {
        if (worldMap != null)
        {
            worldMap.Save(path);
            Debug.LogFormat("ARWorldMap saved to {0}", path);
        }
    }

```


You can load the world map if you know where you saved it:

```CS
        var worldMap = ARWorldMap.Load(path);
```

Once you have the **ARWorldMap**, either from loading it, or from memory, or from receiving it from another device, you can align yourself to share coordinate systems with that **ARWorldMap** by setting a parameter in the config and resetting the ARSession:

```CS
            config.worldMap = worldMap;

            Debug.Log("Restarting session with worldMap");
            session.RunWithConfig(config);
```

What this does is reset the session, and as you move around, it tries to matchup the feature points in the worldmap to the feature points it's detecting in your environment.  When they matchup, it relocalizes your device coordinates to match up with the coordinates that were saved in the **ARWorldMap**.


![alt text] (images/UnityARWorldMap.m4v "UnityARWorldMap scene example")

ARWorldMap can also be serialized to a byte array, and sent across to another device using WiFi, Bluetooth or some other means of sharing.  It can also be deserialized on the other side and used to relocalize the other device to the same world mapping as the first device, so that you can have a shared multiplayer experience.

```
		public byte [] SerializeToByteArray();  //ARWorldMap instance method
		
		public static ARWorldMap SerializeFromByteArray(byte[] mapByteArray);  //ARWorldMap static method

```


## ARReferenceObject and ARObjectAnchor

Similar to **ARReferenceImage** and **ARImageAnchor** that existed in ARKit 1.5, we now have **ARReferenceObject** and **ARObjectAnchor** to do detection of objects.

How to use it in Unity: see the _Examples/ARKit2.0/UnityARObjectAnchor/UnityARObjectAnchor.unity_ for example

Again very similar to **ARReferenceImage**, we're going to set up a **ARReferenceObjectsSetAsset**, which contains references to **ARReferenceObjectAssets**, and add that to the config for **ARSession** so that it tries to detect the **ARReferenceObjects** that correspond to those when in the session.

To create an **ARReferenceObjectAsset**, in the Editor go to your Project view into the directory where you want to create the asset, and bring up the _Create/UnityARKitPlugin/ARReferenceObjectAsset_ menu:

![alt text] (images/ARReferenceObjectAsset_creation.png)

Then in the inspector, when **ARReferenceObjectAsset** is selected, populate the Reference Object field with the raw .arobject file that has also been moved over to a project folder.

![alt text] (images/ARReferenceObject_Inspector.png)

Finally, add a bunch of these **ARReferenceObjectAssets** to a **ARReferenceObjectsSetAsset** created in the same way as above:

![alt text] (images/ARReferenceObjectsSetAsset_Inpsector.png)

Now add a reference to this **ARReferenceObjectsSetAsset** to your **ARKitWorldTrackingSessionConfiguration.detectionObjects** field and start your session.  It should detect the objects that you have specified.


![alt text] (images/UnityObjectAnchor.m4v "UnityObjectAnchor scene example")


You can also look at a more complete example that does object creation using a pickable bounding box and object detection:

_Examples/ARKit2.0/UnityARObjectScanner/UnityARObjectScanner.unity_

You can save objects that you have scanned using this example.  To get the saved objects to your Mac so that you can populate your app with reference objects, you can use iTunes FileSharing while you have your device connected to your Mac:

![alt text] (images/ARReferenceObject_FileShare.png)

Here is a video that shows you this example in action:

![alt text] (images/UnityObjectScanner.m4v "UnityObjectScanner scene example")


## AREnvironmentProbeAnchor

This is a new kind of anchor that can either be generated automatically or you can specify where to create it.  This anchor creates and updates a reflected environment map of the area around it based on the ARKit video frames and world tracking data.

How to use it in Unity: see the _Examples/ARKit2.0/UnityAREnvironmentTexture_ folder for examples

![alt text] (images/UnityEnvironmentAnchor.m4v "UnityAREnvironmentProbeAnchor scene example")

There is a new parameter on the `ARKitWorldTrackingSessionConfiguration` that controls this feature:

```
            config.environmentTexturing = environmentTexturing;
```

where `environmentTexturing` can be one of the values in the enum:

```
	public enum UnityAREnvironmentTexturing
	{
		UnityAREnvironmentTexturingNone,
		UnityAREnvironmentTexturingManual,
		UnityAREnvironmentTexturingAutomatic
	}
```

## ARReferenceImage improvement (now tracked)

Reference images work the same as before, but now they allow you to track them:  when you move the reference image, the Image Anchor associated with them move with the image, so you can have content that is anchored on those moving images.  There is one extra parameter on the `ARKitWorldTrackingSessionConfiguration` that allows you to do this like so:

```
            config.maximumNumberOfTrackedImages = maximumNumberOfTrackedImages;
```

where `maximumNumberOfTrackedImages` is an integer specigying how many images you want tracked simultaenously during this session.

