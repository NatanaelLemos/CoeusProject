$.fn.serializeObject = function () {
    var o = Object.create(null),
        elementMapper = function (element) {
            element.name = $.camelCase(element.name);
            return element;
        },
        appendToResult = function (i, element) {
            var node = o[element.name];

            if ('undefined' != typeof node && node !== null) {
                o[element.name] = node.push ? node.push(element.value) : [node, element.value];
            } else {
                o[element.name] = element.value;
            }
        };

    $.each($.map(this.serializeArray(), elementMapper), appendToResult);
    return o;
};

$.ajaxPost = function (url, data, validatingForm, callBack) {
    if (validatingForm) {
        var validator = $(validatingForm).kendoValidator().data("kendoValidator");
        if (!validator.validate()) {
            return;
        }
    }

    if (!callBack) {
        callBack = function (retUrl) {
            window.location = retUrl;
        };
    }

    $.ajax({
        type: 'POST',
        async: false,
        contentType: "application/json",
        url: url,
        data: JSON.stringify(data),
        success: callBack,
        error: function (error) {
            kendoAlert(error.statusText);
        }
    });
};

function kendoAlert(text, title) {
    if (!title) {
        title = 'Aviso';
    }

    var kendoWindow = $('<div>').kendoWindow({
        title: title,
        resizable: false,
        modal: true,
        actions: {}
    });

    kendoWindow.data('kendoWindow').content(
        text +
        '<br /><br /><button class="k-button" id="btnKendoAlertOk">&nbsp&nbspOK&nbsp&nbsp</button>'
    ).center().open();

    kendoWindow.find("#btnKendoAlertOk").click(function () {
        kendoWindow.data('kendoWindow').close();
    });
}