import { useRouter } from 'next/router';
import PropTypes from 'prop-types';

import { Link } from '../components/Link';
import { usePathname } from 'next/navigation';

export { NavLink };

NavLink.propTypes = {
    href: PropTypes.string.isRequired,
    exact: PropTypes.bool
};

NavLink.defaultProps = {
    exact: false
};

function NavLink({ children, href, exact, ...props }) {
    //const { pathname } = usePathname();
    //console.log(pathname)
    //const isActive = exact ? pathname === href : pathname.startsWith(href);
    
    //if (isActive) {
        //props.className += ' active';
    //}

    return <Link href={href} {...props}>{children}</Link>;
}