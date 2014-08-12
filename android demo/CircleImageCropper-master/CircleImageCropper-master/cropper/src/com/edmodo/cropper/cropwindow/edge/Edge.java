/*
 * Copyright 2013, Edmodo, Inc. 
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this work except in compliance with the License.
 * You may obtain a copy of the License in the LICENSE file, or at:
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" 
 * BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language 
 * governing permissions and limitations under the License. 
*/

package com.edmodo.cropper.cropwindow.edge;

import android.graphics.Rect;
import android.view.View;

import com.edmodo.cropper.util.AspectRatioUtil;

/**
 * Enum representing an edge in the crop window.
 */
public class Edge {

//    LEFT,
//    TOP,
//    RIGHT,
//    BOTTOM;

    // Private Constants ///////////////////////////////////////////////////////
    // Minimum distance in pixels that one edge can get to its opposing edge.
    // This is an arbitrary value that simply prevents the crop window from
    // becoming too small.
    public static final int MIN_CROP_LENGTH_PX = 40;

    // Member Variables ////////////////////////////////////////////////////////

//    private float mCoordinate;

    // Public Methods //////////////////////////////////////////////////////////

    /**
     * Sets the coordinate of the Edge. The coordinate will represent the
     * x-coordinate for LEFT and RIGHT Edges and the y-coordinate for TOP and
     * BOTTOM edges.
     * 
     * @param coordinate the position of the edge
     */
//    public void setCoordinate(float coordinate) {
//        mCoordinate = coordinate;
//    }

    /**
     * Add the given number of pixels to the current coordinate position of this
     * Edge.
     * 
     * @param distance the number of pixels to add
     */
    public static float offset(float distance, float coordinate) {
        return coordinate + distance;
    }

    /**
     * Gets the coordinate of the Edge
     * 
     * @return the Edge coordinate (x-coordinate for LEFT and RIGHT Edges and
     *         the y-coordinate for TOP and BOTTOM edges)
     */
//    public float getCoordinate() {
//        return mCoordinate;
//    }

    /**
     * Sets the Edge to the given x-y coordinate but also adjusting for snapping
     * to the image bounds and parent view border constraints.
     * 
     * @param x the x-coordinate
     * @param y the y-coordinate
     * @param imageRect the bounding rectangle of the image
     * @param imageSnapRadius the radius (in pixels) at which the edge should
     *            snap to the image
     */
    public static float adjustCoordinate(float x, float y, Rect imageRect, float imageSnapRadius, float aspectRatio, EdgeOrientation orientation) {
    	float result = 0;
        switch (orientation) {
            case LEFT:
            	result = adjustLeft(x, imageRect, imageSnapRadius, aspectRatio);
                break;
            case TOP:
            	result = adjustTop(y, imageRect, imageSnapRadius, aspectRatio);
                break;
            case RIGHT:
            	result = adjustRight(x, imageRect, imageSnapRadius, aspectRatio);
                break;
            case BOTTOM:
            	result = adjustBottom(y, imageRect, imageSnapRadius, aspectRatio);
                break;
        }
        return result;
    }
    


    /**
     * Adjusts this Edge position such that the resulting window will have the
     * given aspect ratio.
     * 
     * @param aspectRatio the aspect ratio to achieve
     */
    public static float adjustCoordinate(float aspectRatio, EdgeOrientation orientation) {
    	float result = 0;
        final float left = EdgeType.LEFT;
        final float top = EdgeType.TOP;
        final float right = EdgeType.RIGHT;
        final float bottom = EdgeType.BOTTOM;

        switch (orientation) {
            case LEFT:
                result = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
                break;
            case TOP:
            	result = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
                break;
            case RIGHT:
            	result = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
                break;
            case BOTTOM:
            	result = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);
                break;
        }
        return result;
    }

    /**
     * Returns whether or not you can re-scale the image based on whether any edge would be out of bounds.
     * Checks all the edges for a possibility of jumping out of bounds.
     * 
     * @param Edge the Edge that is about to be expanded
     * @param imageRect the rectangle of the picture
     * @param aspectratio the desired aspectRatio of the picture.
     * 
     * @return whether or not the new image would be out of bounds.
     */
    public static boolean isNewRectangleOutOfBounds(EdgeOrientation edge, Rect imageRect, float aspectRatio, float coordinate,
    		EdgeOrientation orientation) {

        float offset = Edge.snapOffset(imageRect, coordinate, orientation);
        
        switch (orientation) {
            case LEFT:
                if (edge.equals(EdgeOrientation.TOP)) {
                    float top = imageRect.top;
                    float bottom = EdgeType.BOTTOM - offset;
                    float right = EdgeType.RIGHT;
                    float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeOrientation.BOTTOM)) {
                    float bottom = imageRect.bottom;
                    float top = EdgeType.TOP - offset;
                    float right = EdgeType.RIGHT;
                    float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                }
                break;
                
            case TOP:
                if (edge.equals(EdgeOrientation.LEFT)) {
                    float left = imageRect.left;
                    float right = EdgeType.RIGHT - offset;
                    float bottom = EdgeType.BOTTOM;
                    float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeOrientation.RIGHT)) {
                    float right = imageRect.right;
                    float left = EdgeType.LEFT - offset;
                    float bottom = EdgeType.BOTTOM;
                    float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                }
                break; 
                
            case RIGHT:
                if (edge.equals(EdgeOrientation.TOP)) {
                    float top = imageRect.top;
                    float bottom = EdgeType.BOTTOM - offset;
                    float left = EdgeType.LEFT;
                    float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeOrientation.BOTTOM)) {
                    float bottom = imageRect.bottom;
                    float top = EdgeType.TOP - offset;
                    float left = EdgeType.LEFT;
                    float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                }
                break;
                
                
            case BOTTOM:
                if (edge.equals(EdgeOrientation.LEFT)) {
                    float left = imageRect.left;
                    float right = EdgeType.RIGHT - offset;
                    float top = EdgeType.TOP;
                    float bottom = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeOrientation.RIGHT)) {
                    float right = imageRect.right;
                    float left = EdgeType.LEFT - offset;
                    float top = EdgeType.TOP;
                    float bottom = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                break; 
        }
        return true;
    }
    
   /**
    * Returns whether the new rectangle would be out of bounds.
    * 
    * @param top
    * @param left
    * @param bottom
    * @param right
    * @param imageRect the Image to be compared with.
    * @return whether it would be out of bounds
    */
    private static boolean isOutOfBounds(float top, float left, float bottom, float right, Rect imageRect) {
        return (top < imageRect.top || left < imageRect.left || bottom > imageRect.bottom || right > imageRect.right);
    }

    /**
     * Snap this Edge to the given image boundaries.
     * 
     * @param imageRect the bounding rectangle of the image to snap to
     * @return the amount (in pixels) that this coordinate was changed (i.e. the
     *         new coordinate minus the old coordinate value)
     */
    public static float snapToRect(Rect imageRect, float coordinate, EdgeOrientation orientation) {

        float oldCoordinate = coordinate;

        switch (orientation) {
            case LEFT:
            	coordinate = imageRect.left;
                break;
            case TOP:
            	coordinate = imageRect.top;
                break;
            case RIGHT:
            	coordinate = imageRect.right;
                break;
            case BOTTOM:
            	coordinate = imageRect.bottom;
                break;
        }

        final float offset = coordinate - oldCoordinate;
        return offset;
    }
    
    /**
     * Returns the potential snap offset of snaptoRect, without changing the coordinate.
     * 
     * @param imageRect the bounding rectangle of the image to snap to
     * @return the amount (in pixels) that this coordinate was changed (i.e. the
     *         new coordinate minus the old coordinate value)
     */
    public static float snapOffset(Rect imageRect, float coordinate, EdgeOrientation orientation) {

        final float oldCoordinate = coordinate;
        float newCoordinate = oldCoordinate;

        switch (orientation) {
            case LEFT:
                newCoordinate = imageRect.left;
                break;
            case TOP:
                newCoordinate = imageRect.top;
                break;
            case RIGHT:
                newCoordinate = imageRect.right;
                break;
            case BOTTOM:
                newCoordinate = imageRect.bottom;
                break;
        }

        final float offset = newCoordinate - oldCoordinate;
        return offset;
    }

    /**
     * Snap this Edge to the given View boundaries.
     * 
     * @param view the View to snap to
     */
    public static float snapToView(View view, float coordinate, EdgeOrientation orientation) {
    	
        switch (orientation) {
            case LEFT:
            	coordinate = 0;
                break;
            case TOP:
            	coordinate = 0;
                break;
            case RIGHT:
            	coordinate = view.getWidth();
                break;
            case BOTTOM:
            	coordinate = view.getHeight();
                break;
        }
        return coordinate;
    }

    /**
     * Gets the current width of the crop window.
     */
    public static float getWidth() 
    {
        return EdgeType.RIGHT - EdgeType.LEFT;
    }

    /**
     * Gets the current height of the crop window.
     */
    public static float getHeight() 
    {
        return EdgeType.BOTTOM - EdgeType.TOP;
    }

    /**
     * Determines if this Edge is outside the inner margins of the given bounding
     * rectangle. The margins come inside the actual frame by SNAPRADIUS amount; 
     * therefore, determines if the point is outside the inner "margin" frame.
     * 
     */
    public static boolean isOutsideMargin(Rect rect, float margin, float coordinate, EdgeOrientation orientation) {

        boolean result = false;

        switch (orientation) {
            case LEFT:
                result = coordinate - rect.left < margin;
                break;
            case TOP:
                result = coordinate - rect.top < margin;
                break;
            case RIGHT:
                result = rect.right - coordinate < margin;
                break;
            case BOTTOM:
                result = rect.bottom - coordinate < margin;
                break;
        }
        return result;
    }
    
    /**
     * Determines if this Edge is outside the image frame of the given bounding
     * rectangle.
     */
    public static boolean isOutsideFrame(Rect rect, float coordinate, EdgeOrientation orientation) {

        double margin = 0;
        boolean result = false;

        switch (orientation) {
            case LEFT:
                result = coordinate - rect.left < margin;
                break;
            case TOP:
                result = coordinate - rect.top < margin;
                break;
            case RIGHT:
                result = rect.right - coordinate < margin;
                break;
            case BOTTOM:
                result = rect.bottom - coordinate < margin;
                break;
        }
        return result;
    }


    // Private Methods /////////////////////////////////////////////////////////

    /**
     * Get the resulting x-position of the left edge of the crop window given
     * the handle's position and the image's bounding box and snap radius.
     * 
     * @param x the x-position that the left edge is dragged to
     * @param imageRect the bounding box of the image that is being cropped
     * @param imageSnapRadius the snap distance to the image edge (in pixels)
     * @return the actual x-position of the left edge
     */
    private static float adjustLeft(float x, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        float resultX = x;

        if (x - imageRect.left < imageSnapRadius)
            resultX = imageRect.left;

        else
        {
            // Select the minimum of the three possible values to use
            float resultXHoriz = Float.POSITIVE_INFINITY;
            float resultXVert = Float.POSITIVE_INFINITY;

            // Checks if the window is too small horizontally
            if (x >= EdgeType.RIGHT - MIN_CROP_LENGTH_PX)
                resultXHoriz = EdgeType.RIGHT - MIN_CROP_LENGTH_PX;

            // Checks if the window is too small vertically
            if (((EdgeType.RIGHT - x) / aspectRatio) <= MIN_CROP_LENGTH_PX)
                resultXVert = EdgeType.RIGHT - (MIN_CROP_LENGTH_PX * aspectRatio);

            resultX = Math.min(resultX, Math.min(resultXHoriz, resultXVert));
        }
        return resultX;
    }

    /**
     * Get the resulting x-position of the right edge of the crop window given
     * the handle's position and the image's bounding box and snap radius.
     * 
     * @param x the x-position that the right edge is dragged to
     * @param imageRect the bounding box of the image that is being cropped
     * @param imageSnapRadius the snap distance to the image edge (in pixels)
     * @return the actual x-position of the right edge
     */
    private static float adjustRight(float x, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        float resultX = x;

        // If close to the edge
        if (imageRect.right - x < imageSnapRadius)
            resultX = imageRect.right;

        else
        {
            // Select the maximum of the three possible values to use
            float resultXHoriz = Float.NEGATIVE_INFINITY;
            float resultXVert = Float.NEGATIVE_INFINITY;

            // Checks if the window is too small horizontally
            if (x <= EdgeType.LEFT + MIN_CROP_LENGTH_PX)
                resultXHoriz = EdgeType.LEFT + MIN_CROP_LENGTH_PX;

            // Checks if the window is too small vertically
            if (((x - EdgeType.LEFT) / aspectRatio) <= MIN_CROP_LENGTH_PX) {
                resultXVert = EdgeType.LEFT + (MIN_CROP_LENGTH_PX * aspectRatio);
            }

            resultX = Math.max(resultX, Math.max(resultXHoriz, resultXVert));

        }

        return resultX;
    }

    /**
     * Get the resulting y-position of the top edge of the crop window given the
     * handle's position and the image's bounding box and snap radius.
     * 
     * @param y the x-position that the top edge is dragged to
     * @param imageRect the bounding box of the image that is being cropped
     * @param imageSnapRadius the snap distance to the image edge (in pixels)
     * @return the actual y-position of the top edge
     */
    private static float adjustTop(float y, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        float resultY = y;

        if (y - imageRect.top < imageSnapRadius)
            resultY = imageRect.top;

        else
        {
            // Select the minimum of the three possible values to use
            float resultYVert = Float.POSITIVE_INFINITY;
            float resultYHoriz = Float.POSITIVE_INFINITY;

            // Checks if the window is too small vertically
            if (y >= EdgeType.BOTTOM - MIN_CROP_LENGTH_PX)
                resultYHoriz = EdgeType.BOTTOM - MIN_CROP_LENGTH_PX;

            // Checks if the window is too small horizontally
            if (((EdgeType.BOTTOM - y) * aspectRatio) <= MIN_CROP_LENGTH_PX)
                resultYVert = EdgeType.BOTTOM - (MIN_CROP_LENGTH_PX / aspectRatio);

            resultY = Math.min(resultY, Math.min(resultYHoriz, resultYVert));

        }

        return resultY;
    }

    /**
     * Get the resulting y-position of the bottom edge of the crop window given
     * the handle's position and the image's bounding box and snap radius.
     * 
     * @param y the x-position that the bottom edge is dragged to
     * @param imageRect the bounding box of the image that is being cropped
     * @param imageSnapRadius the snap distance to the image edge (in pixels)
     * @return the actual y-position of the bottom edge
     */
    private static float adjustBottom(float y, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        float resultY = y;

        if (imageRect.bottom - y < imageSnapRadius)
            resultY = imageRect.bottom;
        else
        {
            // Select the maximum of the three possible values to use
            float resultYVert = Float.NEGATIVE_INFINITY;
            float resultYHoriz = Float.NEGATIVE_INFINITY;

            // Checks if the window is too small vertically
            if (y <= EdgeType.TOP + MIN_CROP_LENGTH_PX)
                resultYVert = EdgeType.TOP + MIN_CROP_LENGTH_PX;

            // Checks if the window is too small horizontally
            if (((y - EdgeType.TOP) * aspectRatio) <= MIN_CROP_LENGTH_PX)
                resultYHoriz = EdgeType.TOP + (MIN_CROP_LENGTH_PX / aspectRatio);

            resultY = Math.max(resultY, Math.max(resultYHoriz, resultYVert));
        }

        return resultY;
    }
}
