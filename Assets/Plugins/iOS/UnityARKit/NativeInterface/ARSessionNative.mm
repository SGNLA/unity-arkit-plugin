#import <ARKit/ARKit.h>
#import <CoreVideo/CoreVideo.h>
#include "stdlib.h"
#include "UnityAppController.h"


typedef struct
{
    float x,y,z,w;
} UnityARVector4;

typedef struct
{
    UnityARVector4 column0;
    UnityARVector4 column1;
    UnityARVector4 column2;
    UnityARVector4 column3;
} UnityARMatrix4x4;

enum UnityARAlignment
{
    UnityARAlignmentGravity,
    UnityARAlignmentGravityAndHeading,
    UnityARAlignmentCamera
};

enum UnityARPlaneDetection
{
    UnityARPlaneDetectionNone = 0,
    UnityARPlaneDetectionHorizontal = (1 << 0),
    UnityARPlaneDetectionVertical = (1 << 1)
};


typedef struct
{
    UnityARAlignment alignment;
    uint32_t getPointCloudData;
    uint32_t enableLightEstimation;
    
} ARKitSessionConfiguration;

typedef struct
{
    UnityARAlignment alignment;
    UnityARPlaneDetection planeDetection;
    uint32_t getPointCloudData;
    uint32_t enableLightEstimation;

} ARKitWorldTrackingSessionConfiguration;

enum UnityARSessionRunOptions
{
    UnityARSessionRunOptionResetTracking           = (1 << 0),
    UnityARSessionRunOptionRemoveExistingAnchors   = (1 << 1)

};

typedef struct
{
    void* identifier;
    UnityARMatrix4x4 transform;
    ARPlaneAnchorAlignment alignment;
    UnityARVector4 center;
    UnityARVector4 extent;
} UnityARAnchorData;

typedef struct
{
    void* identifier;
    UnityARMatrix4x4 transform;
} UnityARUserAnchorData;

enum UnityARTrackingState
{
    UnityARTrackingStateNotAvailable,
    UnityARTrackingStateLimited,
    UnityARTrackingStateNormal,
};

enum UnityARTrackingReason
{
    UnityARTrackingStateReasonNone,
    UnityARTrackingStateReasonInitializing,
    UnityARTrackingStateReasonExcessiveMotion,
    UnityARTrackingStateReasonInsufficientFeatures,
};

typedef struct
{
    UnityARMatrix4x4 worldTransform;
    UnityARMatrix4x4 projectionMatrix;
    UnityARTrackingState trackingState;
    UnityARTrackingReason trackingReason;
    uint32_t getPointCloudData;
} UnityARCamera;

typedef struct
{
    vector_float3* pointCloud;
    NSUInteger pointCloudSize;
} UnityARPointCloudData;

typedef void (*UNITY_AR_FRAME_CALLBACK)(UnityARCamera camera);
typedef void (*UNITY_AR_ANCHOR_CALLBACK)(UnityARAnchorData anchorData);
typedef void (*UNITY_AR_USER_ANCHOR_CALLBACK)(UnityARUserAnchorData anchorData);
typedef void (*UNITY_AR_SESSION_FAILED_CALLBACK)(void* error);

static inline ARWorldAlignment GetARWorldAlignmentFromUnityARAlignment(UnityARAlignment& unityAlignment)
{
    switch (unityAlignment)
    {
        case UnityARAlignmentGravity:
            return ARWorldAlignmentGravity;
        case UnityARAlignmentGravityAndHeading:
            return ARWorldAlignmentGravityAndHeading;
        case UnityARAlignmentCamera:
            return ARWorldAlignmentCamera;
    }
}

static inline ARPlaneDetection GetARPlaneDetectionFromUnityARPlaneDetection(UnityARPlaneDetection planeDetection)
{
    ARPlaneDetection ret = ARPlaneDetectionNone;
    if ((planeDetection & UnityARPlaneDetectionNone) != 0)
        ret |= ARPlaneDetectionNone;
    if ((planeDetection & UnityARPlaneDetectionHorizontal) != 0)
        ret |= ARPlaneDetectionHorizontal;
    return ret;
}

static inline UnityARTrackingState GetUnityARTrackingStateFromARTrackingState(ARTrackingState trackingState)
{
    switch (trackingState) {
        case ARTrackingStateNormal:
            return UnityARTrackingStateNormal;
        case ARTrackingStateLimited:
            return UnityARTrackingStateLimited;
        case ARTrackingStateNotAvailable:
            return UnityARTrackingStateNotAvailable;
        default:
            [NSException raise:@"UnrecognizedARTrackingState" format:@"Unrecognized ARTrackingState: %ld", (long)trackingState];
            break;
    }
}

static inline UnityARTrackingReason GetUnityARTrackingReasonFromARTrackingReason(ARTrackingStateReason trackingReason)
{
    switch (trackingReason)
    {
        case ARTrackingStateReasonNone:
            return UnityARTrackingStateReasonNone;
        case ARTrackingStateReasonInitializing:
            return UnityARTrackingStateReasonInitializing;
        case ARTrackingStateReasonExcessiveMotion:
            return UnityARTrackingStateReasonExcessiveMotion;
        case ARTrackingStateReasonInsufficientFeatures:
            return UnityARTrackingStateReasonInsufficientFeatures;
        default:
            [NSException raise:@"UnrecognizedARTrackingStateReason" format:@"Unrecognized ARTrackingStateReason: %ld", (long)trackingReason];
            break;
    }
}

inline ARSessionRunOptions GetARSessionRunOptionsFromUnityARSessionRunOptions(UnityARSessionRunOptions runOptions)
{
    ARSessionRunOptions ret = 0;
    if ((runOptions & UnityARSessionRunOptionResetTracking) != 0)
        ret |= ARSessionRunOptionResetTracking;
    if ((runOptions & UnityARSessionRunOptionRemoveExistingAnchors) != 0)
        ret |= ARSessionRunOptionRemoveExistingAnchors;
    return ret;
}

inline void GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(ARKitWorldTrackingSessionConfiguration& unityConfig, ARWorldTrackingSessionConfiguration* appleConfig)
{
    appleConfig.planeDetection = GetARPlaneDetectionFromUnityARPlaneDetection(unityConfig.planeDetection);
    appleConfig.worldAlignment = GetARWorldAlignmentFromUnityARAlignment(unityConfig.alignment);
    appleConfig.lightEstimationEnabled = (BOOL)unityConfig.enableLightEstimation;
}

inline void GetARSessionConfigurationFromARKitSessionConfiguration(ARKitSessionConfiguration& unityConfig, ARSessionConfiguration* appleConfig)
{
    appleConfig.worldAlignment = GetARWorldAlignmentFromUnityARAlignment(unityConfig.alignment);
    appleConfig.lightEstimationEnabled = (BOOL)unityConfig.enableLightEstimation;
}

inline void ARKitMatrixToUnityARMatrix4x4(const matrix_float4x4& matrixIn, UnityARMatrix4x4* matrixOut)
{
    vector_float4 c0 = matrixIn.columns[0];
    matrixOut->column0.x = c0.x;
    matrixOut->column0.y = c0.y;
    matrixOut->column0.z = c0.z;
    matrixOut->column0.w = c0.w;

    vector_float4 c1 = matrixIn.columns[1];
    matrixOut->column1.x = c1.x;
    matrixOut->column1.y = c1.y;
    matrixOut->column1.z = c1.z;
    matrixOut->column1.w = c1.w;

    vector_float4 c2 = matrixIn.columns[2];
    matrixOut->column2.x = c2.x;
    matrixOut->column2.y = c2.y;
    matrixOut->column2.z = c2.z;
    matrixOut->column2.w = c2.w;

    vector_float4 c3 = matrixIn.columns[3];
    matrixOut->column3.x = c3.x;
    matrixOut->column3.y = c3.y;
    matrixOut->column3.z = c3.z;
    matrixOut->column3.w = c3.w;
}

inline void UnityARAnchorDataFromARAnchorPtr(UnityARAnchorData& anchorData, ARPlaneAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
    anchorData.alignment = nativeAnchor.alignment;
    anchorData.center.x = nativeAnchor.center.x;
    anchorData.center.y = nativeAnchor.center.y;
    anchorData.center.z = nativeAnchor.center.z;
    anchorData.extent.x = nativeAnchor.extent.x;
    anchorData.extent.y = nativeAnchor.extent.y;
    anchorData.extent.z = nativeAnchor.extent.z;
}

inline void UnityARUserAnchorDataFromARAnchorPtr(UnityARUserAnchorData& anchorData, ARAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
}

// These don't all need to be static data, but no other better place for them at the moment.
static id <MTLTexture> s_CapturedImageTextureY;
static id <MTLTexture> s_CapturedImageTextureCbCr;
static UnityARMatrix4x4 s_CameraProjectionMatrix;

static float s_AmbientIntensity;
static int s_TrackingQuality;
static float s_ShaderScale;
static const vector_float3* s_PointCloud;
static NSUInteger s_PointCloudSize;

static float unityCameraNearZ;
static float unityCameraFarZ;

@protocol UnityARAnchorEventDispatcher
@required
    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor;
    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor;
    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor;
@end

@interface UnityARAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARAnchorCallbackWrapper

    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorAddedCallback(data);
    }

    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorRemovedCallback(data);
    }

    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorUpdatedCallback(data);
    }

@end

@interface UnityARUserAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARUserAnchorCallbackWrapper

    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorAddedCallback(data);
    }

    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorRemovedCallback(data);
    }

    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorUpdatedCallback(data);
    }

@end

@interface UnityARSession : NSObject <ARSessionDelegate>
{
@public
    ARSession* _session;
    UNITY_AR_FRAME_CALLBACK _frameCallback;
    UNITY_AR_SESSION_FAILED_CALLBACK _arSessionFailedCallback;

    NSMutableDictionary* _classToCallbackMap;
    
    id <MTLDevice> _device;
    CVMetalTextureCacheRef _textureCache;
    BOOL _getPointCloudData;
}
@end

@implementation UnityARSession

- (id)init
{
    if (self = [super init])
    {
        _classToCallbackMap = [[NSMutableDictionary alloc] init];
    }
    return self;
}

- (void)setupMetal
{
    _device = MTLCreateSystemDefaultDevice();
    CVMetalTextureCacheCreate(NULL, NULL, _device, NULL, &_textureCache);
}

- (void)teardownMetal
{
    if (_textureCache) {
        CFRelease(_textureCache);
    }
}

static CGAffineTransform s_CurAffineTransform;

- (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame
{
    s_AmbientIntensity = frame.lightEstimate.ambientIntensity;
    s_TrackingQuality = (int)frame.camera.trackingState;
    s_PointCloud = frame.rawFeaturePoints.points;
    s_PointCloudSize = frame.rawFeaturePoints.count;

    UIInterfaceOrientation orient = [[UIApplication sharedApplication] statusBarOrientation];

    CGRect nativeBounds = [[UIScreen mainScreen] nativeBounds];
    CGSize nativeSize = GetAppController().rootView.bounds.size;

    matrix_float4x4 projectionMatrix = [frame.camera projectionMatrixWithViewportSize:nativeSize orientation:[[UIApplication sharedApplication] statusBarOrientation] zNear:(CGFloat)unityCameraNearZ zFar:(CGFloat)unityCameraFarZ];

    s_CurAffineTransform = [frame displayTransformWithViewportSize:nativeSize orientation:[[UIApplication sharedApplication] statusBarOrientation]];

    UnityARCamera unityARCamera;

    // TODO cgoy: remove s_CameraProjection and send it through callback -> make scripts subscribe to event.
    ARKitMatrixToUnityARMatrix4x4(projectionMatrix, &s_CameraProjectionMatrix);
    ARKitMatrixToUnityARMatrix4x4(projectionMatrix, &unityARCamera.projectionMatrix);

    unityARCamera.trackingState = GetUnityARTrackingStateFromARTrackingState(frame.camera.trackingState);
    unityARCamera.trackingReason = GetUnityARTrackingReasonFromARTrackingReason(frame.camera.trackingStateReason);
    unityARCamera.getPointCloudData = _getPointCloudData;

    if (_frameCallback != NULL)
    {

        matrix_float4x4 rotatedMatrix = matrix_identity_float4x4;

        // rotation  matrix
        // [ cos    -sin]
        // [ sin     cos]
        switch (orient) {
            case UIInterfaceOrientationPortrait:
                rotatedMatrix.columns[0][0] = 0;
                rotatedMatrix.columns[0][1] = 1;
                rotatedMatrix.columns[1][0] = -1;
                rotatedMatrix.columns[1][1] = 0;
                break;
            case UIInterfaceOrientationLandscapeLeft:
                rotatedMatrix.columns[0][0] = -1;
                rotatedMatrix.columns[0][1] = 0;
                rotatedMatrix.columns[1][0] = 0;
                rotatedMatrix.columns[1][1] = -1;
                break;
            case UIInterfaceOrientationPortraitUpsideDown:
                rotatedMatrix.columns[0][0] = 0;
                rotatedMatrix.columns[0][1] = -1;
                rotatedMatrix.columns[1][0] = 1;
                rotatedMatrix.columns[1][1] = 0;
                break;
            default:
                break;
        }

        matrix_float4x4 matrix = matrix_multiply(frame.camera.transform, rotatedMatrix);

        ARKitMatrixToUnityARMatrix4x4(matrix, &unityARCamera.worldTransform);

        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
            _frameCallback(unityARCamera);
        });
    }

    CVPixelBufferRef pixelBuffer = frame.capturedImage;

    size_t imageWidth = CVPixelBufferGetWidth(pixelBuffer);
    size_t imageHeight = CVPixelBufferGetHeight(pixelBuffer);

    float imageAspect = (float)imageWidth / (float)imageHeight;
    float screenAspect = nativeBounds.size.height / nativeBounds.size.width;
    s_ShaderScale = screenAspect / imageAspect;

    if (CVPixelBufferGetPlaneCount(pixelBuffer) < 2 || CVPixelBufferGetPixelFormatType(pixelBuffer) != kCVPixelFormatType_420YpCbCr8BiPlanarFullRange) {
        return;
    }

    id<MTLTexture> textureY = nil;
    id<MTLTexture> textureCbCr = nil;

    // textureY
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 0);
        const size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 0);
        MTLPixelFormat pixelFormat = MTLPixelFormatR8Unorm;

        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, pixelBuffer, NULL, pixelFormat, width, height, 0, &texture);
        if(status == kCVReturnSuccess)
        {
            textureY = CVMetalTextureGetTexture(texture);
            CFRelease(texture);
        }
    }

    // textureCbCr
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 1);
        const size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 1);
        MTLPixelFormat pixelFormat = MTLPixelFormatRG8Unorm;

        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, pixelBuffer, NULL, pixelFormat, width, height, 1, &texture);
        if(status == kCVReturnSuccess)
        {
            textureCbCr = CVMetalTextureGetTexture(texture);
            CFRelease(texture);
        }
    }

    if (textureY != nil && textureCbCr != nil) {
        dispatch_async(dispatch_get_main_queue(), ^{
            // always assign the textures atomic
            s_CapturedImageTextureY = textureY;
            s_CapturedImageTextureCbCr = textureCbCr;
        });
    }
}

- (void)session:(ARSession *)session didFailWithError:(NSError *)error
{
    NSLog(@"AR FAIL");
    if (_arSessionFailedCallback != NULL)
    {
        _arSessionFailedCallback((void*)"ar session failed");
    }
}

- (void)session:(ARSession *)session didAddAnchors:(NSArray<ARAnchor*>*)anchors
{
    NSLog(@"AR ANCHOR ADDED");
    [self sendAnchorAddedEventToUnity:anchors];
}

- (void)session:(ARSession *)session didUpdateAnchors:(NSArray<ARAnchor*>*)anchors
{
    NSLog(@"AR ANCHOR UPDATE");
   [self sendAnchorUpdatedEventToUnity:anchors];
}

- (void)session:(ARSession *)session didRemoveAnchors:(NSArray<ARAnchor*>*)anchors
{
    NSLog(@"AR ANCHOR REMOVED");
   [self sendAnchorRemovedEventToUnity:anchors];
}

- (void) sendAnchorAddedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorAddedEvent:anchorPtr];
    }
}

- (void) sendAnchorRemovedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorRemovedEvent:anchorPtr];
    }
}

- (void) sendAnchorUpdatedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorUpdatedEvent:anchorPtr];
    }
}

@end

/// Create the native mirror to the C# ARSession object

extern "C" void* unity_CreateNativeARSession(UNITY_AR_FRAME_CALLBACK frameCallback, 
                                            UNITY_AR_ANCHOR_CALLBACK anchorAddedCallback, 
                                            UNITY_AR_ANCHOR_CALLBACK anchorUpdatedCallback, 
                                            UNITY_AR_ANCHOR_CALLBACK anchorRemovedCallback, 
                                            UNITY_AR_USER_ANCHOR_CALLBACK userAnchorAddedCallback, 
                                            UNITY_AR_USER_ANCHOR_CALLBACK userAnchorUpdatedCallback, 
                                            UNITY_AR_USER_ANCHOR_CALLBACK userAnchorRemovedCallback, 
                                            UNITY_AR_SESSION_FAILED_CALLBACK sessionFailed)
{
    UnityARSession *nativeSession = [[UnityARSession alloc] init];
    nativeSession->_session = [ARSession new];
    nativeSession->_session.delegate = nativeSession;
    nativeSession->_frameCallback = frameCallback;

    UnityARAnchorCallbackWrapper* anchorCallbacks = [[UnityARAnchorCallbackWrapper alloc] init];
    anchorCallbacks->_anchorAddedCallback = anchorAddedCallback;
    anchorCallbacks->_anchorUpdatedCallback = anchorUpdatedCallback;
    anchorCallbacks->_anchorRemovedCallback = anchorRemovedCallback;

    UnityARUserAnchorCallbackWrapper* userAnchorCallbacks = [[UnityARUserAnchorCallbackWrapper alloc] init];
    userAnchorCallbacks->_anchorAddedCallback = userAnchorAddedCallback;
    userAnchorCallbacks->_anchorUpdatedCallback = userAnchorUpdatedCallback;
    userAnchorCallbacks->_anchorRemovedCallback = userAnchorRemovedCallback;

    [nativeSession->_classToCallbackMap setObject:anchorCallbacks forKey:[ARPlaneAnchor class]];
    [nativeSession->_classToCallbackMap setObject:userAnchorCallbacks forKey:[ARAnchor class]];

    nativeSession->_arSessionFailedCallback = sessionFailed;
    unityCameraNearZ = .01;
    unityCameraFarZ = 30;
    return (__bridge_retained void*)nativeSession;
}

extern "C" void StartWorldTrackingSessionWithOptions(void* nativeSession, ARKitWorldTrackingSessionConfiguration unityConfig, UnityARSessionRunOptions runOptions)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARWorldTrackingSessionConfiguration* config = [ARWorldTrackingSessionConfiguration new];
    ARSessionRunOptions runOpts = GetARSessionRunOptionsFromUnityARSessionRunOptions(runOptions);
    GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config options:runOpts ];
    [session setupMetal];
}



extern "C" void StartWorldTrackingSession(void* nativeSession, ARKitWorldTrackingSessionConfiguration unityConfig)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARWorldTrackingSessionConfiguration* config = [ARWorldTrackingSessionConfiguration new];
    GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config];
    [session setupMetal];
}

extern "C" void StartSessionWithOptions(void* nativeSession, ARKitSessionConfiguration unityConfig, UnityARSessionRunOptions runOptions)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARSessionConfiguration* config = [ARSessionConfiguration new];
    ARSessionRunOptions runOpts = GetARSessionRunOptionsFromUnityARSessionRunOptions(runOptions);
    GetARSessionConfigurationFromARKitSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config options:runOpts ];
    [session setupMetal];
}



extern "C" void StartSession(void* nativeSession, ARKitSessionConfiguration unityConfig)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARSessionConfiguration* config = [ARSessionConfiguration new];
    GetARSessionConfigurationFromARKitSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config];
    [session setupMetal];
}

extern "C" void PauseSession(void* nativeSession)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    [session->_session pause];
}

extern "C" void StopSession(void* nativeSession)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    [session teardownMetal];
}

extern "C" UnityARAnchorData SessionAddAnchor(void* nativeSession, UnityARAnchorData anchorData)
{
    // create a native ARAnchor and add it to the session
    // then return the data back to the user that they will
    // need in case they want to remove it
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARAnchor *newAnchor = [[ARAnchor alloc] initWithTransform:matrix_identity_float4x4];
    
    [session->_session addAnchor:newAnchor];
    UnityARAnchorData returnAnchorData;
    UnityARAnchorDataFromARAnchorPtr(returnAnchorData, newAnchor);
    return returnAnchorData;
}

extern "C" void SessionRemoveAnchor(void* nativeSession, const char * anchorIdentifier)
{
    // go through anchors and find the right one
    // then remove it from the session
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    for (ARAnchor* a in session->_session.currentFrame.anchors)
    {
        if ([[a.identifier UUIDString] isEqualToString:[NSString stringWithUTF8String:anchorIdentifier]])
        {
            [session->_session removeAnchor:a];
            return;
        }
    }
}

extern "C" void SetCameraNearFar (float nearZ, float farZ)
{
    unityCameraNearZ = nearZ;
    unityCameraFarZ = farZ;
}

extern "C" struct HitTestResult
{
    void* ptr;
    int count;
};

// Must match ARHitTestResult in ARHitTestResult.cs
extern "C" struct UnityARHitTestResult
{
    ARHitTestResultType type;
    double distance;
    UnityARMatrix4x4 localTransform;
    UnityARMatrix4x4 worldTransform;
    void* anchorPtr;
    bool isValid;
};

// Must match ARTextureHandles in UnityARSession.cs
extern "C" struct UnityARTextureHandles
{
    void* textureY;
    void* textureCbCr;
};

// Cache results locally
static NSArray<ARHitTestResult *>* s_LastHitTestResults;

// Returns the number of hits and caches the results internally
extern "C" int HitTest(void* nativeSession, CGPoint point, ARHitTestResultType types)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;

    // this is what i want to use! why can't i?
     point = CGPointApplyAffineTransform(CGPointMake(point.x, 1.0f - point.y), CGAffineTransformInvert(s_CurAffineTransform));
     s_LastHitTestResults = [session->_session.currentFrame hitTest:point types:types];

    return (int)[s_LastHitTestResults count];
}
extern "C" UnityARHitTestResult GetLastHitTestResult(int index)
{
    UnityARHitTestResult unityResult;
    memset(&unityResult, 0, sizeof(UnityARHitTestResult));

    if (s_LastHitTestResults != nil && index >= 0 && index < [s_LastHitTestResults count])
    {
        ARHitTestResult* hitResult = s_LastHitTestResults[index];
        unityResult.type = hitResult.type;
        unityResult.distance = hitResult.distance;
        ARKitMatrixToUnityARMatrix4x4(hitResult.localTransform, &unityResult.localTransform);
        ARKitMatrixToUnityARMatrix4x4(hitResult.worldTransform, &unityResult.worldTransform);
        unityResult.anchorPtr = (void*)[hitResult.anchor.identifier.UUIDString UTF8String];
        unityResult.isValid = true;
    }

    return unityResult;
}

extern "C" UnityARTextureHandles GetVideoTextureHandles()
{
    UnityARTextureHandles handles;
    handles.textureY = (__bridge_retained void*)s_CapturedImageTextureY;
    handles.textureCbCr = (__bridge_retained void*)s_CapturedImageTextureCbCr;

    return handles;
}

extern "C" bool GetARPointCloud(float** verts, unsigned int* vertLength)
{
    *verts = (float*)s_PointCloud;
    *vertLength = (unsigned int)s_PointCloudSize * 4;
    return YES;
}

extern "C" UnityARMatrix4x4 GetCameraProjectionMatrix()
{
    return s_CameraProjectionMatrix;
}

extern "C" float GetAmbientIntensity()
{
    return s_AmbientIntensity;
}

extern "C" int GetTrackingQuality()
{
    return s_TrackingQuality;
}

extern "C" float GetYUVTexCoordScale()
{
    return s_ShaderScale;
}

extern "C" bool IsARKitWorldTrackingSessionConfigurationSupported()
{
    return ARWorldTrackingSessionConfiguration.isSupported;
}

extern "C" bool IsARKitSessionConfigurationSupported()
{
    return ARSessionConfiguration.isSupported;
}
