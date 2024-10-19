import { Asset, Watchable, Property } from '@intuiface/core';

@Asset({
    name: 'HeadPoseEstimation',
    category: 'Face Detection',
    behaviors: []
  })
export class HeadPoseEstimation extends Watchable {
  /**
   * Head pitch angle
   */
  @Property({
    displayName: 'Pitch',
    description: 'The pitch angle of the head.',
    defaultValue: 0,
    type: Number,
  })
  public pitch: number = 0;

  /**
   * Head yaw angle
   */
  @Property({
    displayName: 'Yaw',
    description: 'The yaw angle of the head.',
    defaultValue: 0,
    type: Number,
  })
  public yaw: number = 0;

  /**
   * Head roll angle
   */
  @Property({
    displayName: 'Roll',
    description: 'The roll angle of the head.',
    defaultValue: 0,
    type: Number,
  })
  public roll: number = 0;

  constructor() {
    super();
  }
}
