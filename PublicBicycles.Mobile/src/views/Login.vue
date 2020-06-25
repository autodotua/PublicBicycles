<template>
  <div class="login">
    <div class="back-img"></div>
    <div v-title data-title="登录注册 - 公共自行车"></div>
    <div class="title">公共自行车
    </div>
    <div class="box">
      <el-input id="name" v-model="username" placeholder="请输入帐号">
        <template slot="prepend">帐号</template>
      </el-input>

      <br />
      <el-input id="password" v-model="password" type="password" placeholder="请输入密码">
        <template slot="prepend">密码</template>
      </el-input>

      <br />
      <br />

      <el-button
        id="login"
        v-on:click="login"
        :disabled="buttonsDisabled"
        style="width:100%"
        type="primary"
      >登录</el-button>

      <br />
      <br />
      <el-button id="login" v-on:click="register" :disabled="buttonsDisabled" style="width:100%">注册</el-button>

      <!-- <el-link href="register" >注册</el-link> -->
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import { Notification } from "element-ui";
import { showError, getUrl, jump } from "../common";
import { AxiosResponse } from "axios";
export default Vue.extend({
  data: function() {
    return { username: "admin", password: "admin", buttonsDisabled: false };
  },
  methods: {
    /**
     * 登陆后，设置cookies
     */
    afterLogin(response: AxiosResponse<any>, register = false) {
      if (response.data.succeed) {
        Cookies.set("userID", response.data.data.userID);
        Cookies.set("token", response.data.data.token);
        Cookies.set("username", response.data.data.user.username);
        Cookies.set("isAdmin", response.data.data.user.isAdmin);
        jump("");
      } else {
        Notification.error(
          (register ? "注册失败：" : "登陆失败：") + response.data.message
        );
      }
    },
    /**
     * 登录
     */
    login() {
      Vue.axios
        .post(getUrl("User", "Login"), {
          UserName: this.username,
          Password: this.password
        })
        .then(response => {
          this.afterLogin(response, false);
        })
        .catch(showError);
    },
    /**
     * 注册
     */
    register() {
      Vue.axios
        .post(getUrl("User", "Register"), {
          UserName: this.username,
          Password: this.password
        })
        .then(response => {
          this.afterLogin(response, true);
        })
        .catch(showError);
    }
  }
});
</script>

<style scoped>
.box {
  position: absolute;
  width: calc(100% - 24px);
  top: 30%;
  right: 12px;
  left: 12px;
}
.box .el-row {
  width: 100%;
}

.login {
  width: 100%;
  height: 100%;
}

h1 {
}

.back-img {
  width: 100%;
  height: 100%;
  opacity: 0.4;
  background-image: url(/img/login_background.jpg);
  background-size: cover;
  background-repeat: no-repeat;
  background-position: center center;
  position: absolute;
  top: 0;
  z-index: -1;
}

.title{
  text-align: center; 
  color: white;
  -webkit-text-stroke-width: 0.2rem;
  -webkit-text-stroke-color: black ;
  font-size: 3rem;
  font-weight: bold;
  margin-top: 1rem;
  /* margin-left:10%;
  margin-right:10%;
  margin-top:6px;
  padding-top:6px;
  padding-bottom:6px; */
}
div{
  overflow: hidden;
}
</style>