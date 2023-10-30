import { useEffect } from 'react';
import { useRouter } from 'next/navigation';

import { userService } from '../../services/user.service';

export { Layout };

function Layout({ children }) {
    // const router = useRouter();

    // useEffect(() => {
    //     // redirect to home if already logged in
    //     if (userService.userValue) {
    //         router.push('/');
    //     }

    //     // eslint-disable-next-line react-hooks/exhaustive-deps
    // }, []);

    return (
        <div className="flex items-center justify-center min-h-screen transition-colors overflow-hidden bg-gradient-to-r from-gunmetaldark via-gunmetallight to-gunmetallighter duration-300">
            {children}
        </div>
    );
}