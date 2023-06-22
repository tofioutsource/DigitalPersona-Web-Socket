package org.dp.models;

import com.digitalpersona.uareu.Reader;

public class DPReaderModel {

    public DPReaderModel(Reader.Id id, String name) {
        this.id = id;
        this.name = name;
    }

    private Reader.Id id;
    private String name;

    public Reader.Id getId() {
        return id;
    }

    public String getName() {
        return name;
    }
}
