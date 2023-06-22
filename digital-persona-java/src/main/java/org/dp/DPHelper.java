package org.dp;

import com.digitalpersona.uareu.Fid;

import java.util.Base64;

public class DPHelper {

    public static String imageToBase64(Fid image) {
        Fid.Fiv view = image.getViews()[0];
        String b64 = Base64.getEncoder().encodeToString(view.getImageData());
        return "data:image/jpeg;base64, %s".formatted(b64);
    }

}
