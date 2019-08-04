var crypto=require("crypto-js");

function generateHash(value){
    var data=JSON.stringify(value);
    var sha256=crypto.SHA256(data).toString();
    return sha256;
}

const data1 = "Li Hui";
let hash=generateHash(data1);
console.log(hash);
