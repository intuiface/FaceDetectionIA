// import { Action, Asset, IntuifaceElement, Parameter, Property, Trigger } from '@intuiface/core';

// /**
//  * Custom Interface Asset FaceDetection
//  */
// @Asset({
//     name: 'FaceDetection',
//     category: 'FaceDetection',
//     behaviors: [],
// })
// export class FaceDetection extends IntuifaceElement {

//     //#region Public Properties

//     /**
//      * Property example
//      */
//     @Property({
//         displayName: 'propertyExample',
//         description: 'A property declaration example.',
//         defaultValue: 0,
//         minValue: 0,
//         maxValue: 10,
//         type: Number
//     })
//     public propertyExample: number = 0;

//     //#endregion Public Properties

//     //#region Constructor
//     /**
//      * @constructor
//      */
//     public constructor()
//     {
//         super();
//     }

//     //#endregion Constructor

//     //#region Triggers

//     /**
//      * Trigger Example
//      */
//     @Trigger({
//         name: 'exampleTrigger',
//         displayName: 'A Trigger Example',
//         description: 'Raised when the property example changed'
//     })
//     public exampleTrigger(
//         @Parameter({
//             name: 'triggerParam',
//             displayName: 'Trigger parameter',
//             description: 'A trigger parameter example.',
//             defaultValue: '',
//             type: String
//         }) triggerParam: string
//     ): void {}

//     //#endregion Triggers

//     //#region Actions

//     /**
//      * Action Example
//      */
//     @Action({
//         displayName: 'Action Example',
//         description: 'An Action example with a parameter and validation',
//         validate: true
//     })
//     public actionExample(
//         @Parameter({
//             name: 'actionParam',
//             displayName: 'Action parameter',
//             description: 'An action parameter example.',
//             defaultValue: 1,
//             minValue: 0,
//             maxValue: 10,
//             type: Number
//         }) actionParam: number): void
//     {
//         if (this.propertyExample !== actionParam) {
//             this.propertyExample = actionParam;
//             // raise the trigger
//             this.exampleTrigger('An example parameter string value');
//         }
//     }
//     //#endregion Actions
// }


import { Asset, IntuifaceElement, Property, Action, Trigger, Parameter } from '@intuiface/core';
import { Face } from './Face';
import { EmotionConfidence } from './EmotionConfidence';
import { HeadPoseEstimation } from './HeadPostEstimation';


// Main FaceDetection asset
@Asset({
    name: 'FaceDetectionTS',
    displayName: 'Face Detection with OpenVINO - TypeScript',
    category: 'Face Detection',
    description: 'Detects faces and returns information about them.',
    behaviors: [],
})
export class FaceDetectionTS extends IntuifaceElement {
    @Property({ displayName: 'Server Host', type: String })
    serverHost: string = '127.0.0.1';

    @Property({ displayName: 'Server Port', type: Number })
    serverPort: number = 2975;

    @Property({ displayName: 'Face Count', type: Number, readOnly: true })
    faceCount: number = 0;

    @Property({ displayName: 'Activity Log', type: String, readOnly: true })
    activityLog: string = '';

    @Property({ displayName: 'Minimum Face Size', type: Number })
    minimumFaceSize: number = 100;

    @Property({ displayName: 'Detection Update Frequency', type: Number })
    detectionUpdateFrequency: number = 100;

    @Property({ displayName: 'Is Connected to Face Detection Server', type: Boolean, readOnly: true })
    isConnectedToFaceDetectionServer: boolean = false;

    @Property({ displayName: 'Is Main Face Detected', type: Boolean, readOnly: true })
    isMainFaceDetected: boolean = false;

    @Property({ displayName: 'Main Face', type: Face, readOnly: true })
    mainFace: Face = new Face();

    @Property({ displayName: 'Faces', type: Array, itemType: Face, readOnly: true })
    faces: Face[] = [];


    private webSocket: WebSocket;

    // Action to connect to the server
    @Action({ displayName: 'Connect to Server' })
    connectToServer() {
        this.activityLog += `Trying to open WebSocket on ${this.serverHost}:${this.serverPort}\n`;

        const webSocketUrl = `ws://${this.serverHost}:${this.serverPort}`;

        try {
            // Create a new WebSocket connection
            this.webSocket = new WebSocket(webSocketUrl);

            // Set up event handlers
            this.webSocket.onopen = (event) => {
                this.isConnectedToFaceDetectionServer = true;
                this.activityLog += 'WebSocket connection opened.\n';
            };

            this.webSocket.onmessage = (event) => {
                // Process incoming messages (e.g., detected faces)
                const data = JSON.parse(event.data);
                if (data.faces) {
                    // Update the list of faces and other relevant properties
                    this._updateFacesList(data.faces);
                }
            };

            this.webSocket.onerror = (error) => {
                this.activityLog += `WebSocket error: ${error}\n`;
            };

            this.webSocket.onclose = (event) => {
                this.isConnectedToFaceDetectionServer = false;
                this.activityLog += `WebSocket closed: ${event.reason}\n`;
            };

        } catch (error) {
            this.activityLog += `Error connecting to WebSocket: ${error.message}\n`;
        }
    }

    private _updateFacesList(facesData: any[]) {
        const newFaces: Face[] = [];
        let mainFaceID = -1;
        let maxFaceSize = -1;

        facesData.forEach((faceData) => {
            const id = faceData.id;
            const gender = faceData.gender;
            const age = faceData.age;
            const ageRange = this._getAgeRange(parseInt(age, 10));
            const x = Number(faceData.location.x) + Number(faceData.location.width) / 2;

            const y = Number(faceData.location.y) + (faceData.location.height) / 2;
            const width = Number(faceData.location.width);
            const height = Number(faceData.location.height);
            const faceSize = width * height;
            const mainEmotion = faceData.mainEmotion.emotion;
            const mainEmotionConfidence = Number(faceData.mainEmotion.confidence);

            // Parse additional emotions

            let emotionConfidence = new EmotionConfidence();
            emotionConfidence.angry = Number(faceData.emotions.anger); // float.Parse(v["emotions"]["anger"].ToString(), CultureInfo.InvariantCulture);
            emotionConfidence.happy = Number(faceData.emotions.happy);//float.Parse(v["emotions"]["happy"].ToString(), CultureInfo.InvariantCulture);
            emotionConfidence.neutral = Number(faceData.emotions.neutral);// float.Parse(v["emotions"]["neutral"].ToString(), CultureInfo.InvariantCulture);
            emotionConfidence.sad = Number(faceData.emotions.sad);// float.Parse(v["emotions"]["sad"].ToString(), CultureInfo.InvariantCulture);
            emotionConfidence.surprised = Number(faceData.emotions.surprise); //float.Parse(v["emotions"]["surprise"].ToString(), CultureInfo.InvariantCulture);

            // Parse head pose estimation
            let headPoseEstimation = new HeadPoseEstimation();
            headPoseEstimation.pitch = Number(faceData.headpose.pitch);//float.Parse(v["headpose"]["pitch"].ToString(), CultureInfo.InvariantCulture);
            headPoseEstimation.yaw = Number(faceData.headpose.yaw);//float.Parse(v["headpose"]["yaw"].ToString(), CultureInfo.InvariantCulture);
            headPoseEstimation.roll = Number(faceData.headpose.roll);//float.Parse(v["headpose"]["roll"].ToString(), CultureInfo.InvariantCulture);

            //TODO If face size filtering is active, check is head is bigger than threshold
            // if (faceSize < m_dMinimumFaceSize) {
            //     // Don't add face to list
            //     continue;
            // }

            // TODO: use real "distance" and take the min one. 
            if (faceSize > maxFaceSize) {
                maxFaceSize = faceSize;
                mainFaceID = id;
            }

            const newFace = new Face({
                id: id,
                gender: gender,
                age: age,
                ageRange: ageRange,
                x: x,
                y: y,
                width: width,
                height: height,
                faceSize: faceSize,
                mainEmotion: mainEmotion,
                mainEmotionConfidence: mainEmotionConfidence,
                emotionConfidence: emotionConfidence,
                headPoseEstimation: headPoseEstimation
            });

            newFaces.push(newFace);
            this.notifyPropertyChanged('Faces', this.faces);

        });

        this.faces = newFaces.sort((a, b) => a.id - b.id);
        this.faceCount = this.faces.length;

        const mainFace = this.faces.find(face => face.id === mainFaceID);
        if (mainFace) {
            this.mainFace = mainFace;
            this.isMainFaceDetected = true;
        } else {
            this.isMainFaceDetected = false;
        }
    }

    private _getAgeRange(age: number): string {
        if (age <= 16) return 'child';
        if (age <= 30) return 'young adult';
        if (age <= 45) return 'middle-aged adult';
        return 'old-aged adult';
    }


    // Action to disconnect from the server
    @Action({ displayName: 'Disconnect from Server' })
    disconnectFromServer() {
        this.activityLog += `Trying to close WebSocket on ${this.serverHost}:${this.serverPort}\n`;

        try {
            // Close the WebSocket connection if it exists and is open
            if (this.webSocket && this.webSocket.readyState === WebSocket.OPEN) {
                this.webSocket.close();
                this.activityLog += 'WebSocket connection closed.\n';
            } else {
                this.activityLog += 'No active WebSocket connection to close.\n';
            }

            // Update connection status
            this.isConnectedToFaceDetectionServer = false;
        } catch (error) {
            // Handle any errors that might occur during disconnection
            this.activityLog += `Error while closing WebSocket: ${error.message}\n`;
        }
    }

    // Trigger when a face is detected
    @Trigger({ name: 'FaceDetected', displayName: 'Face Detected' })
    onFaceDetected(@Parameter({ name: 'Face', displayName: 'Detected Face', type: Object }) face: Face) {
        // Trigger logic when a face is detected
    }

    // Trigger when a face is lost
    @Trigger({ name: 'FaceLost', displayName: 'Face Lost' })
    onFaceLost(@Parameter({ name: 'Face', displayName: 'Lost Face', type: Object }) face: Face) {
        // Trigger logic when a face is lost
    }

    // Trigger when the face count changes
    @Trigger({ name: 'FaceCountChanged', displayName: 'Face Count Changed' })
    onFaceCountChanged(@Parameter({ name: 'Count', displayName: 'New Face Count', type: Number }) count: number) {
        // Trigger logic when face count changes
    }
}
