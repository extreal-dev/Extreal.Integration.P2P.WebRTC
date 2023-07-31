import { callback, addAction, addFunction } from "@extreal-dev/extreal.integration.web.common";

addAction("DoAction", (str1, str2) => {
    callback("HandleOnCallback", `param1=[${str1}]`, `param2=[${str2}]`);
});

addFunction("DoFunction", (str1, str2) => {
    return `received param1=[${str1}] param2=[${str2}] in function`;
});
