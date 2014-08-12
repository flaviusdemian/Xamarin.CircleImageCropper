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

package com.edmodo.cropper.cropwindow.handle;

import android.graphics.Rect;

import com.edmodo.cropper.cropwindow.edge.EdgeOrientation;
import com.edmodo.cropper.cropwindow.edge.EdgeType;

/**
 * Enum representing a pressable, draggable Handle on the crop window.
 */
public enum Handle {

    TOP_LEFT(new CornerHandleHelper(EdgeOrientation.TOP, EdgeOrientation.LEFT, EdgeType.TOP, EdgeType.LEFT)),
    TOP_RIGHT(new CornerHandleHelper(EdgeOrientation.TOP, EdgeOrientation.RIGHT, EdgeType.TOP, EdgeType.RIGHT)),
    BOTTOM_LEFT(new CornerHandleHelper(EdgeOrientation.BOTTOM, EdgeOrientation.LEFT, EdgeType.BOTTOM, EdgeType.LEFT)),
    BOTTOM_RIGHT(new CornerHandleHelper(EdgeOrientation.BOTTOM, EdgeOrientation.RIGHT, EdgeType.BOTTOM, EdgeType.RIGHT)),
    LEFT(new VerticalHandleHelper(EdgeOrientation.LEFT, EdgeType.LEFT)),
    TOP(new HorizontalHandleHelper(EdgeOrientation.TOP, EdgeType.TOP)),
    RIGHT(new VerticalHandleHelper(EdgeOrientation.RIGHT, EdgeType.RIGHT)),
    BOTTOM(new HorizontalHandleHelper(EdgeOrientation.BOTTOM, EdgeType.BOTTOM)),
    CENTER(new CenterHandleHelper());

    // Member Variables ////////////////////////////////////////////////////////

    private HandleHelper mHelper;

    // Constructors ////////////////////////////////////////////////////////////

    Handle(HandleHelper helper) {
        mHelper = helper;
    }

    // Public Methods //////////////////////////////////////////////////////////

    public void updateCropWindow(float x,
                                 float y,
                                 Rect imageRect,
                                 float snapRadius) {

        mHelper.updateCropWindow(x, y, imageRect, snapRadius);
    }

    public void updateCropWindow(float x,
                                 float y,
                                 float targetAspectRatio,
                                 Rect imageRect,
                                 float snapRadius) {

        mHelper.updateCropWindow(x, y, targetAspectRatio, imageRect, snapRadius);
    }
}
