import cookie from 'wwp-js-cookie'

const TOKEN_KEY = 'qb_token_key';

const getToken = ()=>{
    let ck = cookie.get(TOKEN_KEY);
    return ck ? JSON.parse(ck) :
    {"access_token":"ParvNhecHMmrCYSkNwTjROjJnTGv5rqIg7F1jvfkGa0","expires_in":3600,"token_type":"Bearer","refresh_token":"rNoWZY2JSI9keLCjsZxC5kop7HGeORBpo8r9VgLblNs","rember":false,"token_time":1584785176.564};
}

const setToken = (token)=>{
    cookie.set(TOKEN_KEY,JSON.stringify(token),0,cookie.getRootHost());
}

const removeToken = ()=>{
    cookie.set(TOKEN_KEY,'',-1,cookie.getRootHost());
}

export {getToken,setToken,removeToken};