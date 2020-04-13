import CryptoJS  from 'crypto-js'

const decode = (word,key,iv)=> {
    key =  CryptoJS.enc.Utf8.parse(key);
    iv =  CryptoJS.enc.Utf8.parse(iv);
    let encryptedHexStr = CryptoJS.enc.Hex.parse(word);
    let srcs = CryptoJS.enc.Base64.stringify(encryptedHexStr);
    let decrypt = CryptoJS.AES.decrypt(srcs, key, { iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
    let decryptedStr = decrypt.toString(CryptoJS.enc.Utf8);
    return  decryptedStr.toString();
}
const encode = (word,key,iv)=> {
    key =  CryptoJS.enc.Utf8.parse(key);
    iv =  CryptoJS.enc.Utf8.parse(iv);
    let srcs = CryptoJS.enc.Utf8.parse(word);
    let encrypted = CryptoJS.AES.encrypt(srcs, key, { iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
    return encrypted.ciphertext.toString().toUpperCase();
}
const getKey = ()=>{
    return '1234567890ABCDEF'.split('').sort(()=>{
        return Math.random() > .5 ? -1 : 1
    }).join('');
}
export {encode,decode,getKey}