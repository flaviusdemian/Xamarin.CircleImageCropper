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
public class Edge 
{
    // Private Constants ///////////////////////////////////////////////////////

    // Minimum distance in pixels that one edge can get to its opposing edge.
    // This is an arbitrary value that simply prevents the crop window from
    // becoming too small.
    public static final int MIN_CROP_LENGTH_PX = 40;

    // Member Variables ////////////////////////////////////////////////////////

    public float coordinate;
    public int edgeType;
    // Public Methods //////////////////////////////////////////////////////////
    
    public Edge(int edgeType)
    {
    	this.edgeType = edgeType;
    }
    
    /**
     * Add the given number of pixels to the current coordinate position of this
     * Edge.
     * 
     * @param distance the number of pixels to add
     */
    public void offset(float distance) {
        coordinate += distance;
    }

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
    public void adjustCoordinate(float x, float y, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        switch (this.edgeType)
        {
            case EdgeType.LEFT:
                coordinate = adjustLeft(x, imageRect, imageSnapRadius, aspectRatio);
                break;
            case EdgeType.TOP:
                coordinate = adjustTop(y, imageRect, imageSnapRadius, aspectRatio);
                break;
            case EdgeType.RIGHT:
                coordinate = adjustRight(x, imageRect, imageSnapRadius, aspectRatio);
                break;
            case EdgeType.BOTTOM:
                coordinate = adjustBottom(y, imageRect, imageSnapRadius, aspectRatio);
                break;
        }
    }
    


    /**
     * Adjusts this Edge position such that the resulting window will have the
     * given aspect ratio.
     * 
     * @param aspectRatio the aspect ratio to achieve
     */
    public void adjustCoordinate(float aspectRatio) 
    {
        final float left = EdgeManager.LEFT.coordinate;
        final float top = EdgeManager.TOP.coordinate;
        final float right = EdgeManager.RIGHT.coordinate;
        final float bottom = EdgeManager.BOTTOM.coordinate;

        switch (edgeType) {
            case EdgeType.LEFT:
                coordinate = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
                break;
            case EdgeType.TOP:
                coordinate = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
                break;
            case EdgeType.RIGHT:
                coordinate = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
                break;
            case EdgeType.BOTTOM:
                coordinate = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);
                break;
        }
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
    public boolean isNewRectangleOutOfBounds(Edge edge, Rect imageRect, float aspectRatio) 
    {
        float offset = edge.snapOffset(imageRect);
        
        switch (edgeType) {
            case EdgeType.LEFT:
                if (edge.equals(EdgeManager.TOP)) {
                    float top = imageRect.top;
                    float bottom = EdgeManager.BOTTOM.coordinate - offset;
                    float right = EdgeManager.RIGHT.coordinate;
                    float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeManager.BOTTOM)) {
                    float bottom = imageRect.bottom;
                    float top = EdgeManager.TOP.coordinate - offset;
                    float right = EdgeManager.RIGHT.coordinate;
                    float left = AspectRatioUtil.calculateLeft(top, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                }
                break;
                
            case EdgeType.TOP:
                if (edge.equals(EdgeManager.LEFT)) {
                    float left = imageRect.left;
                    float right = EdgeManager.RIGHT.coordinate - offset;
                    float bottom = EdgeManager.BOTTOM.coordinate;
                    float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeManager.RIGHT)) {
                    float right = imageRect.right;
                    float left = EdgeManager.LEFT.coordinate - offset;
                    float bottom = EdgeManager.BOTTOM.coordinate;
                    float top = AspectRatioUtil.calculateTop(left, right, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                }
                break; 
                
            case EdgeType.RIGHT:
                if (edge.equals(EdgeManager.TOP)) {
                    float top = imageRect.top;
                    float bottom = EdgeManager.BOTTOM.coordinate - offset;
                    float left = EdgeManager.LEFT.coordinate;
                    float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeManager.BOTTOM)) {
                    float bottom = imageRect.bottom;
                    float top = EdgeManager.TOP.coordinate - offset;
                    float left = EdgeManager.LEFT.coordinate;
                    float right = AspectRatioUtil.calculateRight(left, top, bottom, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                }
                break;
                
                
            case EdgeType.BOTTOM:
                if (edge.equals(EdgeManager.LEFT)) {
                    float left = imageRect.left;
                    float right = EdgeManager.RIGHT.coordinate - offset;
                    float top = EdgeManager.TOP.coordinate;
                    float bottom = AspectRatioUtil.calculateBottom(left, top, right, aspectRatio);
                    
                    return isOutOfBounds(top, left, bottom, right, imageRect);
                    
                }
                else if (edge.equals(EdgeManager.RIGHT)) {
                    float right = imageRect.right;
                    float left = EdgeManager.LEFT.coordinate - offset;
                    float top = EdgeManager.TOP.coordinate;
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
    private boolean isOutOfBounds(float top, float left, float bottom, float right, Rect imageRect) {
        return (top < imageRect.top || left < imageRect.left || bottom > imageRect.bottom || right > imageRect.right);
    }

    /**
     * Snap this Edge to the given image boundaries.
     * 
     * @param imageRect the bounding rectangle of the image to snap to
     * @return the amount (in pixels) that this coordinate was changed (i.e. the
     *         new coordinate minus the old coordinate value)
     */
    public float snapToRect(Rect imageRect) {

        final float oldCoordinate = coordinate;

        switch (edgeType) {
            case EdgeType.LEFT:
                coordinate = imageRect.left;
                break;
            case EdgeType.TOP:
                coordinate = imageRect.top;
                break;
            case EdgeType.RIGHT:
                coordinate = imageRect.right;
                break;
            case EdgeType.BOTTOM:
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
    public float snapOffset(Rect imageRect) {

        final float oldCoordinate = coordinate;
        float newCoordinate = oldCoordinate;

        switch (edgeType) {
            case EdgeType.LEFT:
                newCoordinate = imageRect.left;
                break;
            case EdgeType.TOP:
                newCoordinate = imageRect.top;
                break;
            case EdgeType.RIGHT:
                newCoordinate = imageRect.right;
                break;
            case EdgeType.BOTTOM:
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
    public void snapToView(View view) {

        switch (edgeType) {
            case EdgeType.LEFT:
                coordinate = 0;
                break;
            case EdgeType.TOP:
                coordinate = 0;
                break;
            case EdgeType.RIGHT:
                coordinate = view.getWidth();
                break;
            case EdgeType.BOTTOM:
                coordinate = view.getHeight();
                break;
        }
    }

    /**
     * Gets the current width of the crop window.
     */
    public static float getWidth() {
        return EdgeManager.RIGHT.coordinate - EdgeManager.LEFT.coordinate;
    }

    /**
     * Gets the current height of the crop window.
     */
    public static float getHeight() {
        return EdgeManager.BOTTOM.coordinate - EdgeManager.TOP.coordinate;
    }

    /**
     * Determines if this Edge is outside the inner margins of the given bounding
     * rectangle. The margins come inside the actual frame by SNAPRADIUS amount; 
     * therefore, determines if the point is outside the inner "margin" frame.
     * 
     */
    public boolean isOutsideMargin(Rect rect, float margin) {

        boolean result = false;

        switch (edgeType) {
            case EdgeType.LEFT:
                result = coordinate - rect.left < margin;
                break;
            case EdgeType.TOP:
                result = coordinate - rect.top < margin;
                break;
            case EdgeType.RIGHT:
                result = rect.right - coordinate < margin;
                break;
            case EdgeType.BOTTOM:
                result = rect.bottom - coordinate < margin;
                break;
        }
        return result;
    }
    
    /**
     * Determines if this Edge is outside the image frame of the given bounding
     * rectangle.
     */
    public boolean isOutsideFrame(Rect rect) {

        double margin = 0;
        boolean result = false;

        switch (edgeType) {
            case EdgeType.LEFT:
                result = coordinate - rect.left < margin;
                break;
            case EdgeType.TOP:
                result = coordinate - rect.top < margin;
                break;
            case EdgeType.RIGHT:
                result = rect.right - coordinate < margin;
                break;
            case EdgeType.BOTTOM:
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
    private float adjustLeft(float x, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        float resultX = x;

        if (x - imageRect.left < imageSnapRadius)
            resultX = imageRect.left;

        else
        {
            // Select the minimum of the three possible values to use
            float resultXHoriz = Float.POSITIVE_INFINITY;
            float resultXVert = Float.POSITIVE_INFINITY;

            // Checks if the window is too small horizontally
            if (x >= EdgeManager.RIGHT.coordinate - MIN_CROP_LENGTH_PX)
                resultXHoriz = EdgeManager.RIGHT.coordinate - MIN_CROP_LENGTH_PX;

            // Checks if the window is too small vertically
            if (((EdgeManager.RIGHT.coordinate - x) / aspectRatio) <= MIN_CROP_LENGTH_PX)
                resultXVert = EdgeManager.RIGHT.coordinate - (MIN_CROP_LENGTH_PX * aspectRatio);

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
    private float adjustRight(float x, Rect imageRect, float imageSnapRadius, float aspectRatio) {

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
            if (x <= EdgeManager.LEFT.coordinate + MIN_CROP_LENGTH_PX)
                resultXHoriz = EdgeManager.LEFT.coordinate + MIN_CROP_LENGTH_PX;

            // Checks if the window is too small vertically
            if (((x - EdgeManager.LEFT.coordinate) / aspectRatio) <= MIN_CROP_LENGTH_PX) {
                resultXVert = EdgeManager.LEFT.coordinate + (MIN_CROP_LENGTH_PX * aspectRatio);
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
    private float adjustTop(float y, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        float resultY = y;

        if (y - imageRect.top < imageSnapRadius)
            resultY = imageRect.top;

        else
        {
            // Select the minimum of the three possible values to use
            float resultYVert = Float.POSITIVE_INFINITY;
            float resultYHoriz = Float.POSITIVE_INFINITY;

            // Checks if the window is too small vertically
            if (y >= EdgeManager.BOTTOM.coordinate - MIN_CROP_LENGTH_PX)
                resultYHoriz = EdgeManager.BOTTOM.coordinate - MIN_CROP_LENGTH_PX;

            // Checks if the window is too small horizontally
            if (((EdgeManager.BOTTOM.coordinate - y) * aspectRatio) <= MIN_CROP_LENGTH_PX)
                resultYVert = EdgeManager.BOTTOM.coordinate - (MIN_CROP_LENGTH_PX / aspectRatio);

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
    private float adjustBottom(float y, Rect imageRect, float imageSnapRadius, float aspectRatio) {

        float resultY = y;

        if (imageRect.bottom - y < imageSnapRadius)
            resultY = imageRect.bottom;
        else
        {
            // Select the maximum of the three possible values to use
            float resultYVert = Float.NEGATIVE_INFINITY;
            float resultYHoriz = Float.NEGATIVE_INFINITY;

            // Checks if the window is too small vertically
            if (y <= EdgeManager.TOP.coordinate + MIN_CROP_LENGTH_PX)
                resultYVert = EdgeManager.TOP.coordinate + MIN_CROP_LENGTH_PX;

            // Checks if the window is too small horizontally
            if (((y - EdgeManager.TOP.coordinate) * aspectRatio) <= MIN_CROP_LENGTH_PX)
                resultYHoriz = EdgeManager.TOP.coordinate + (MIN_CROP_LENGTH_PX / aspectRatio);

            resultY = Math.max(resultY, Math.max(resultYHoriz, resultYVert));
        }

        return resultY;
    }
}
