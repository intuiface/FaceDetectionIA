import {Asset, Watchable, Property } from '@intuiface/core';

@Asset({
  name: 'EmotionConfidence',
  category: 'Face Detection',
  behaviors: []
})
export class EmotionConfidence extends Watchable {
  /**
   * Angry emotion confidence level
   */
  @Property({
    displayName: 'Angry',
    description: 'Confidence level for Angry emotion.',
    defaultValue: 0,
    type: Number,
  })
  public angry: number = 0;

  /**
   * Surprised emotion confidence level
   */
  @Property({
    displayName: 'Surprised',
    description: 'Confidence level for Surprised emotion.',
    defaultValue: 0,
    type: Number,
  })
  public surprised: number = 0;

  /**
   * Happy emotion confidence level
   */
  @Property({
    displayName: 'Happy',
    description: 'Confidence level for Happy emotion.',
    defaultValue: 0,
    type: Number,
  })
  public happy: number = 0;

  /**
   * Neutral emotion confidence level
   */
  @Property({
    displayName: 'Neutral',
    description: 'Confidence level for Neutral emotion.',
    defaultValue: 0,
    type: Number,
  })
  public neutral: number = 0;

  /**
   * Sad emotion confidence level
   */
  @Property({
    displayName: 'Sad',
    description: 'Confidence level for Sad emotion.',
    defaultValue: 0,
    type: Number,
  })
  public sad: number = 0;

  constructor() {
    super();
  }
}
