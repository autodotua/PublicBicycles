import Cookies from "js-cookie"
import { Notification } from "element-ui"
/**
 * 获取附加了用户ID和Token的对象
 * @param obj 需要附加的对象
 */
export function withToken(obj: object): object {
    const request = {
        UserID: Number.parseInt(Cookies.get("userID") ?? "0"),
        Token: Cookies.get("token")
    }
    Object.assign(request, obj);
    console.log("send request", request);
    return request;
}
/**
 * 判断是否为管理员
 */
export function isAdmin() {
    console.log("is Admin", Cookies.get("isAdmin") === "true")
    return Cookies.get("isAdmin") === "true";
}

/**
 * 格式化日期时间
 * @param time 时间
 * @param includeTime 是否包含时间部分
 */
export function formatDateTime(time: Date | string, includeTime = true): string {
    if (typeof time == "string") {
        time = new Date(time);
    }
    time = time as Date;
    let str = time.getFullYear().toString().padStart(4, '0') + "-"
        + (time.getMonth() + 1).toString().padStart(2, '0') + "-"
        + time.getDate().toString().padStart(2, '0');
    if (includeTime) {
        str += " "
            + time.getHours().toString().padStart(2, '0') + ":"
            + time.getMinutes().toString().padStart(2, '0');
    }
    return str;
}

/**
 * 返回API的URL
 * @param controller 控制器名称
 * @param action 操作名称
 */
export function getUrl(controller: string, action: string) {
    return `http://localhost:8520/${controller}/${action}`;
}

/**
 * 弹出错误框
 * @param r 错误内容 
 */
export function showError(r: any) {
    Notification.error({ title: "错误", message: r });
    console.log("error",r);
}

/**
 * 弹出提示信息
 * @param msg 
 */
export function showNotify(msg: any) {
    Notification.info({ title: "提示", message: msg });
}

/**
 * 跳转到某个地址
 * @param url 
 */
export function jump(url: string) {
    window.location.href = process.env.BASE_URL + "#/" + url;
}