import { Asset, Property, Watchable, } from '@intuiface/core';
import { EmotionConfidence } from './EmotionConfidence';
import { HeadPoseEstimation } from './HeadPostEstimation';

// Define the Face class that is a counterpart to the Face C# class

@Asset({
    name: 'Face',
    category: 'Face Detection',
    behaviors: []
})
export class Face extends Watchable {
    @Property({ displayName: 'Id', type: Number })
    id: number = -1;

    @Property({ displayName: 'Gender', type: String })
    gender: string = '';

    @Property({ displayName: 'Age', type: String })
    age: string = '';

    @Property({ displayName: 'Age Range', type: String })
    ageRange: string = '';

    @Property({ displayName: 'Dwell Time', type: Number })
    dwellTime: number = 0;

    @Property({ displayName: 'X', type: Number })
    x: number = 0;

    @Property({ displayName: 'Y', type: Number })
    y: number = 0;

    @Property({ displayName: 'Width', type: Number })
    width: number = 0;

    @Property({ displayName: 'Height', type: Number })
    height: number = 0;

    @Property({ displayName: 'Face Size', type: Number })
    faceSize: number = 0;

    @Property({ displayName: 'Main Emotion', type: String })
    mainEmotion: string = '';

    @Property({ displayName: 'Main Emotion Confidence', type: Number })
    mainEmotionConfidence: number = 0;

    @Property({ displayName: 'Main Emotion Confidence', type: EmotionConfidence })
    emotionConfidence: EmotionConfidence;

    @Property({ displayName: 'Head Pose Estimation', type: HeadPoseEstimation })
    headPoseEstimation: HeadPoseEstimation;


    constructor(init?: Partial<Face>) {
        super();
        Object.assign(this, init);
    }
}