import React, { userState,  useEffect, useContext } from 'react'

const AuthContext = React.createContext()

const useAuth = () => {
    return useContext(AuthContext);
}

const AuthProvider = (props) => {
    const [authUser, setAuthUser] = useState(null)
    const [isLoggedIn, SetIsLoggedIn] = useState(false)

    const value = {
        authUser,
        setAuthUser,
        isLoggedIn,
        SetIsLoggedIn
    }

    return <AuthContext.Provider value={value}>{props.children}</AuthContext.Provider>
}

export default {useAuth, AuthProvider}